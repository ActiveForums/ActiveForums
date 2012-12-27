if exists (select * from dbo.sysobjects where id = object_id(N'{databaseOwner}{objectQualifier}activeforums_Search_ManageFullText') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure {databaseOwner}{objectQualifier}activeforums_Search_ManageFullText
GO
if exists (select * from dbo.sysobjects where id = object_id(N'{databaseOwner}{objectQualifier}activeforums_Search_FullText') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure {databaseOwner}{objectQualifier}activeforums_Search_FullText
GO
CREATE PROCEDURE {databaseOwner}{objectQualifier}activeforums_Search_ManageFullText
@Enable bit
AS
DECLARE @FullTextEnabled bit
If @Enable = 1
	BEGIN
	
	SET @FullTextEnabled = 0
	
	DECLARE @dbName nvarchar(1000)
	SET @dbName = DB_NAME()
	SET @FullTextEnabled = (SELECT DATABASEPROPERTY(@dbName, 'IsFulltextEnabled'))
	If @FullTextEnabled = 0
		BEGIN
			exec sp_fulltext_database 'enable'
			SET @FullTextEnabled = (SELECT DATABASEPROPERTY(@dbName, 'IsFulltextEnabled'))
		END
	IF @FullTextEnabled > 0
		BEGIN
			IF OBJECTPROPERTY(object_id('{databaseOwner}{objectQualifier}activeforums_Content'),'TableHasActiveFulltextIndex') = 0
				BEGIN
					
					IF  NOT EXISTS (SELECT * FROM sysfulltextcatalogs ftc WHERE ftc.name = N'{objectQualifier}activeforums_Catalog')
						BEGIN
							exec sp_fulltext_catalog '{objectQualifier}activeforums_Catalog', 'create'
						END					
					exec sp_fulltext_table N'{databaseOwner}{objectQualifier}activeforums_Content', N'create', N'{objectQualifier}activeforums_Catalog', N'PK_{objectQualifier}activeforums_Content'
					exec sp_fulltext_column N'{databaseOwner}{objectQualifier}activeforums_Content', N'Subject', N'add', 1033  
					exec sp_fulltext_column N'{databaseOwner}{objectQualifier}activeforums_Content', N'Body', N'add', 1033  
					exec sp_fulltext_table N'{databaseOwner}{objectQualifier}activeforums_Content', N'activate'
					exec sp_fulltext_catalog '{objectQualifier}activeforums_Catalog', 'start_full'
				END
		END
	SELECT @FullTextEnabled
	END
ELSE
	BEGIN
			IF OBJECTPROPERTY(object_id('{databaseOwner}{objectQualifier}activeforums_Content'),'TableHasActiveFulltextIndex') = 1
				BEGIN
					exec sp_fulltext_table N'{databaseOwner}{objectQualifier}activeforums_Content', N'drop'					
				END
		SELECT @Enable
	END
GO
CREATE PROCEDURE {databaseOwner}{objectQualifier}activeforums_Search_FullText
	@PortalId int,
	@ModuleId int,
	@UserId int,
	@ForumId int,
	@IsSuperUser bit,
	@RowIndex int = 0,
	@MaxRows int = 20,
	@SearchString nvarchar(200), 
	@MatchType int = 0,
	@SearchField int = 0,--0=Subject&Body, 1= Subject, 2=Body
	@Timespan int = 0,
	@AuthorId int = 0,
	@Author nvarchar(200),
	@Forums varchar(8000),
	@Tags nvarchar(400),
	@ForumsAllowed nvarchar(1000)
as
DECLARE @BodyRank int
DECLARE @SubjectRank int
SET @BodyRank = 1
SET @SubjectRank = 1
If @SearchField = 1
	SET @BodyRank = NULL
If @SearchField = 2
	SET @SubjectRank = NULL
IF @AuthorId = 0 AND @Author != ''
	BEGIN
		DECLARE @DisplayOpt varchar(50)
		SELECT @DisplayOpt = SettingValue FROM {databaseOwner}{objectQualifier}activeforums_Settings WHERE ModuleId = @ModuleId AND SettingName = 'USERNAMEDISPLAY'
		If @DisplayOpt = 'Fullname' 
			SET @DisplayOpt = 'FirstName  + '' '' + LastName '
		DECLARE @sql nvarchar(2000)
		SET @sql = N'SELECT @RET = UserId FROM {databaseOwner}{objectQualifier}Users WHERE ' + @DisplayOpt + ' = ''' + @Author + ''''
		print @sql
		exec sp_executesql @stmt = @sql, @params = N'@RET as INT OUTPUT', @ret = @AuthorId OUTPUT;
		if @AuthorId = 0 SET @AuthorId = -1
	END
DECLARE @RowCount int
--DECLARE @tmpResults TABLE (resultid int identity(1,1),topicid int, matchpct decimal(15,4))
DECLARE @tmpResults TABLE (resultid int identity(1,1),topicid int,contentid int, matchpct decimal(15,4))
IF @SearchField = 0
	BEGIN
INSERT INTO @tmpResults(topicid,contentid, matchpct)			
	Select c.topicid,c.contentid, b.[RANK] as MatchPct
			FROM {databaseOwner}{objectQualifier}vw_activeforums_TopicView C INNER JOIN
			{databaseOwner}{objectQualifier}activeforums_Functions_Split(@ForumsAllowed,';') as fs ON fs.id = c.ForumId INNER JOIN
			FREETEXTTABLE({databaseOwner}{objectQualifier}activeforums_Content, (Body,[Subject]), @SearchString,200) as B ON C.ContentId = B.[KEY]
			WHERE c.ModuleId = @ModuleId
			
	END
IF @SearchField = 1
BEGIN
INSERT INTO @tmpResults(topicid,contentid, matchpct)			
	Select c.topicid,c.contentid, b.[RANK] as MatchPct
			FROM {databaseOwner}{objectQualifier}vw_activeforums_TopicView C INNER JOIN
			{databaseOwner}{objectQualifier}activeforums_Functions_Split(@ForumsAllowed,';') as fs ON fs.id = c.ForumId INNER JOIN
			FREETEXTTABLE({databaseOwner}{objectQualifier}activeforums_Content, ([Subject]), @SearchString,200) as B ON C.ContentId = B.[KEY]
			WHERE c.ModuleId = @ModuleId


			
	END
IF @SearchField = 2
BEGIN
			INSERT INTO @tmpResults(topicid,contentid, matchpct)
	Select c.topicid,c.contentid, b.[RANK] as MatchPct
			FROM {databaseOwner}{objectQualifier}vw_activeforums_TopicView C INNER JOIN
			{databaseOwner}{objectQualifier}activeforums_Functions_Split(@ForumsAllowed,';') as fs ON fs.id = c.ForumId INNER JOIN
			FREETEXTTABLE({databaseOwner}{objectQualifier}activeforums_Content, (Body), @SearchString,200) as B ON C.ContentId = B.[KEY]
			WHERE c.ModuleId = @ModuleId


			
	END

--declare @topics TABLE (topicid int unique, matchpct decimal(15,4), rownum int)
declare @topics TABLE (topicid int,contentid int, matchpct decimal(15,4), rownum int)
INSERT INTO @topics(topicid,contentid, matchpct,rownum)
		SELECT hits.TopicId,hits.ContentId, hits.MatchPct, ROW_NUMBER() OVER (ORDER BY DateCreated DESC, hits.MatchPct DESC) as rownum FROM
				(
					SELECT  t.topicid,t.datecreated,t.contentid			
					FROM         {databaseOwner}{objectQualifier}vw_activeforums_TopicView AS T INNER JOIN
					{databaseOwner}{objectQualifier}activeforums_Content AS C ON T.ContentId = C.ContentId 
					WHERE 
						
						(@TimeSpan = 0 OR DATEDIFF(hh,T.DateCreated,GetDate()) <= @TimeSpan) AND
						(@AuthorId = 0 OR T.AuthorId = @AuthorId) AND
						(@ForumId <= 0 OR T.ForumId =  @ForumId) AND
						(@Tags = '' OR (@Tags <> '' AND T.TopicId IN (
														SELECT TopicId FROM {databaseOwner}{objectQualifier}activeforums_Tags INNER JOIN
														{databaseOwner}{objectQualifier}activeforums_Topics_Tags ON {databaseOwner}{objectQualifier}activeforums_Tags.TagId = {databaseOwner}{objectQualifier}activeforums_Topics_Tags.TagId
														WHERE	{databaseOwner}{objectQualifier}activeforums_Tags.TagName = @Tags))) AND
						(@SearchString <> '' OR @Tags <> '') AND
						
						(@Forums = '' OR T.ForumId IN (SELECT id FROM {databaseOwner}{objectQualifier}activeforums_Functions_Split(@Forums,':')))
			) as results INNER JOIN @tmpResults as hits ON results.contentid = hits.contentid
SELECT Count(*) from @topics
SELECT T.PortalId, T.ModuleId, T.ForumId, T.ForumName, T.TopicId, T.ReplyId, IsNull(T.Subject,'') as Subject,
	 T.Summary, T.AuthorId, IsNull(T.AuthorName,'') as AuthorName, IsNull(T.Username,'') as UserName, IsNull(T.FirstName,'') as FirstName, 
		IsNull(T.LastName,'') as LastName, IsNull(T.DisplayName,'') as DisplayName, T.DateCreated, 
                      T.DateUpdated, T.ContentId, TopicIcon, StatusId, TopicType, IsPinned, IsLocked, ViewCount, ReplyCount,IsNull(c.Body,'') as Body FROM         
			{databaseOwner}{objectQualifier}vw_activeforums_TopicView AS T INNER JOIN
					@topics AS r ON T.contentid = r.contentid INNER JOIN
					{databaseOwner}{objectQualifier}activeforums_Content as c ON c.ContentId = t.ContentId
WHERE rownum > @RowIndex AND rownum <= (@RowIndex + @MaxRows)
ORDER BY  T.DateCreated DESC,MatchPct DESC
