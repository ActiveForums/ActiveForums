
if exists (select * from {databaseOwner}{objectQualifier}sysobjects where id = object_id(N'{databaseOwner}{objectQualifier}activeforums_Search_ManageFullText') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure {databaseOwner}{objectQualifier}activeforums_Search_ManageFullText
GO
if exists (select * from {databaseOwner}{objectQualifier}sysobjects where id = object_id(N'{databaseOwner}{objectQualifier}activeforums_Search_FullText') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure {databaseOwner}{objectQualifier}activeforums_Search_FullText
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}activeforums_Search_ManageFullText]
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
					
					IF  NOT EXISTS (SELECT * FROM sysfulltextcatalogs ftc WHERE ftc.name = N'activeforums_Catalog')
						BEGIN
							exec sp_fulltext_catalog 'activeforums_Catalog', 'create'
						END					
					exec sp_fulltext_table N'{databaseOwner}{objectQualifier}activeforums_Content', N'create', N'activeforums_Catalog', N'PK_activeforums_Content'
					exec sp_fulltext_column N'{databaseOwner}{objectQualifier}activeforums_Content', N'Subject', N'add', 1033  
					exec sp_fulltext_column N'{databaseOwner}{objectQualifier}activeforums_Content', N'Body', N'add', 1033  
					exec sp_fulltext_table N'{databaseOwner}{objectQualifier}activeforums_Content', N'activate'
					exec sp_fulltext_table '{databaseOwner}{objectQualifier}activeforums_Content', 'start_change_tracking'
					exec sp_fulltext_table '{databaseOwner}{objectQualifier}activeforums_Content', 'start_background_updateindex'
					--exec sp_fulltext_catalog 'activeforums_Catalog', 'start_full'
					
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





USE [DNN7]
GO

/****** Object:  StoredProcedure {databaseOwner}[{objectQualifier}activeforums_Search_FullText]    Script Date: 03/14/2013 22:52:33 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}activeforums_Search_FullText]

	@PortalId int,
	@ModuleId int,
	@UserId int,
	@SearchString nvarchar(200), -- String of 1 or more search terms, all separated by spaces
	@MatchType int = 0, -- 0 = match any, 1 = match all, 2 = exact match of entire expression only
	@SearchField int = 0, -- 0 = Subject & Body, 1 = Subject, 2 =Body
	@Timespan int = 0,
	@AuthorId int = 0,
	@Forums nvarchar(max), -- Intersection of forums allowed and forums requested
	@Tags nvarchar(400), -- Comma delmited tags
	@ResultType int = 0, -- 0 = topics, 1 = posts
	@Sort int = 0 -- 0 = relevance then post date (last), 1 = post date (last)

AS

-- Temp table to store our full text search results


-- Parse out the Words

DECLARE @Word nvarchar(200)
DECLARE @WordTable table (Word nvarchar(200) NOT NULL)
DECLARE @WordCount int = 0

IF @SearchString IS NOT NULL AND @SearchString <> ''
BEGIN
	IF(@MatchType = 2)
		INSERT INTO @WordTable VALUES(@SearchString) 
	ELSE
		INSERT INTO @WordTable
		SELECT string
		FROM {databaseOwner}{objectQualifier}activeforums_Functions_SplitText(@SearchString, ',')
	
	SET @WordCount = (SELECT COUNT(*) from @WordTable)
END

-- If we dont' have any words, no point in doing the search
IF @WordCount = 0
BEGIN
	DECLARE @emptyResults TABLE (rn int, tid int, cid int, mcpt decimal(15,4))
	SELECT * FROM @emptyResults
	RETURN
END


-- Parse out the Tags

DECLARE @Tag nvarchar(400)
DECLARE @TagTable table (Tag nvarchar(400) NOT NULL)
DECLARE @TagCount int = 0

IF @Tags IS NOT NULL AND @Tags <> ''
BEGIN
	INSERT INTO @TagTable
	SELECT string
	FROM {databaseOwner}{objectQualifier}activeforums_Functions_SplitText(@Tags, ',')
	
	SET @TagCount = (SELECT COUNT(*) from @TagTable)
END

-- Build our contains statement

DECLARE @Contains nvarchar(4000) = ''
DECLARE @Delimiter nvarchar(5) = ' OR ';
DECLARE @CurrentWord nvarchar(200) = NULL

IF @MatchType = 1
	SET @Delimiter = ' AND '

DECLARE WordCursor CURSOR FOR SELECT Word FROM @WordTable
OPEN WordCursor
	FETCH NEXT FROM WordCursor INTO @CurrentWord
	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF @Contains <> ''
			SET @Contains = @Contains + @Delimiter
		
		SET @Contains = @Contains + '"' + @CurrentWord + '"'	
			
		FETCH NEXT FROM WordCursor INTO @CurrentWord
	END
CLOSE WordCursor
DEALLOCATE WordCursor

SELECT *
INTO #forums
FROM {databaseOwner}{objectQualifier}activeforums_Functions_Split(@Forums,':')

-- Grab our full text results

DECLARE @tmpResults TABLE (tid int, cid int, mcpt decimal(15,4))	

IF @SearchField = 0
BEGIN
		INSERT INTO @tmpResults
		SELECT c.topicid as tid ,c.contentid as cid, b.[RANK] as mcpt
		FROM {databaseOwner}{objectQualifier}vw_activeforums_TopicView C INNER JOIN
			#forums as f on C.ForumId = f.id INNER JOIN
			CONTAINSTABLE({databaseOwner}{objectQualifier}activeforums_Content, (Body,[Subject]), @Contains, 1000) as B ON C.ContentId = B.[KEY]
		WHERE c.ModuleId = @ModuleId			
END
IF @SearchField = 1
BEGIN
		INSERT INTO @tmpResults
		SELECT c.topicid as tid ,c.contentid as cid, b.[RANK] as mcpt
		FROM {databaseOwner}{objectQualifier}vw_activeforums_TopicView C INNER JOIN
			#forums as f on C.ForumId = f.id INNER JOIN
			CONTAINSTABLE({databaseOwner}{objectQualifier}activeforums_Content, ([Subject]), @Contains, 1000) as B ON C.ContentId = B.[KEY]
		WHERE c.ModuleId = @ModuleId	
END
IF @SearchField = 2

		INSERT INTO @tmpResults
		SELECT c.topicid as tid ,c.contentid as cid, b.[RANK] as mcpt
		FROM {databaseOwner}{objectQualifier}vw_activeforums_TopicView C INNER JOIN
			#forums as f on C.ForumId = f.id INNER JOIN
			CONTAINSTABLE({databaseOwner}{objectQualifier}activeforums_Content, (Body), @Contains, 1000) as B ON C.ContentId = B.[KEY]
		WHERE c.ModuleId = @ModuleId

SELECT *
INTO #tmpResults
FROM @tmpResults

IF @ResultType = 1
BEGIN

	-- Get our main result set
	SELECT TOP 1000 
		ROW_NUMBER() OVER (ORDER BY CASE @Sort WHEN 1 THEN DateCreated ELSE hits.mcpt END DESC, DateCreated DESC) as rn, 
		TopicId as tid, 
		ContentId as cid, 
		hits.mcpt as mcpt
	FROM (
			SELECT  t.topicid,
				 t.contentid, 
				 c.DateCreated	
			FROM {databaseOwner}{objectQualifier}vw_activeforums_TopicView AS T INNER JOIN 
				{databaseOwner}{objectQualifier}activeforums_Content AS C ON T.ContentId = C.ContentId
			WHERE T.PortalId = @PortalId AND T.ModuleId = @ModuleId AND 
				(@TimeSpan = 0 OR DATEDIFF(hh,c.DateCreated,GetDate()) <= @TimeSpan) AND
				(@AuthorId = 0 OR T.AuthorId = @AuthorId) AND
				(@TagCount = 0 OR  T.TopicId IN (
					SELECT TopicId FROM {databaseOwner}{objectQualifier}activeforums_Tags INNER JOIN
						{databaseOwner}{objectQualifier}activeforums_Topics_Tags ON {databaseOwner}{objectQualifier}activeforums_Tags.TagId = {databaseOwner}{objectQualifier}activeforums_Topics_Tags.TagId INNER JOIN
						@TagTable TT ON TT.Tag = {databaseOwner}{objectQualifier}activeforums_Tags.TagName)) 
		) AS results INNER JOIN
			#tmpResults as hits ON results.contentid = hits.cid

	RETURN	
END

IF @ResultType = 0
BEGIN

	-- Get our main result set
	SELECT TOP 1000 
		ROW_NUMBER() OVER (ORDER BY CASE @Sort WHEN 1 THEN MAX(LastReplyDate) ELSE SUM(hits.mcpt) END DESC, MAX(LastReplyDate) DESC) as rn, 
		TopicId as tid, 
		MAX(ContentId) as cid, 
		SUM(hits.mcpt) as mcpt
	FROM (
			SELECT  t.topicid, 
				t.contentid, 
				CASE WHEN rc.DateCreated IS NULL THEN c.DateCreated ELSE rc.DateCreated END  as LastReplyDate		
			FROM {databaseOwner}{objectQualifier}vw_activeforums_TopicView AS T INNER JOIN 
				{databaseOwner}{objectQualifier}activeforums_ForumTopics FT on T.TopicId = FT.TopicId INNER JOIN
				{databaseOwner}{objectQualifier}activeforums_Content AS C ON T.ContentId = C.ContentId  LEFT OUTER JOIN -- Left outer joins to get last reply date
				{databaseOwner}{objectQualifier}activeforums_Replies as R on FT.LastReplyId = r.ReplyId LEFT OUTER JOIN
				{databaseOwner}{objectQualifier}activeforums_Content as RC on R.ContentId = rc.ContentId 
			WHERE T.PortalId = @PortalId AND T.ModuleId = @ModuleId AND 
			(@TimeSpan = 0 OR DATEDIFF(hh,CASE WHEN rc.DateCreated IS NULL THEN c.DateCreated ELSE rc.DateCreated END,GetDate()) <= @TimeSpan) AND
			(@AuthorId = 0 OR T.AuthorId = @AuthorId) AND
			(@TagCount = 0 OR  T.TopicId IN (
				SELECT TopicId FROM {databaseOwner}{objectQualifier}activeforums_Tags INNER JOIN
					{databaseOwner}{objectQualifier}activeforums_Topics_Tags ON {databaseOwner}{objectQualifier}activeforums_Tags.TagId = {databaseOwner}{objectQualifier}activeforums_Topics_Tags.TagId INNER JOIN
					@TagTable TT ON TT.Tag = {databaseOwner}{objectQualifier}activeforums_Tags.TagName))
		) AS results INNER JOIN
			#tmpResults as hits ON results.contentid = hits.cid
	GROUP BY TopicID

	RETURN	
END


GO

