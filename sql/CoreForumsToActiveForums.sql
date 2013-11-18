/*********************************************************/
/*													     */
/*     V1.5 - 9-30-2012								     */
/*     Enhancements by Chris Hammond				     */
/*     DotNetNuke Corporation						     */
/*     Additional Enhancements by					     */
/*     Matthias Schloman - Aarsys.de				     */
/*     (c) 2013 DNN Corporation							 */
/*     http://activeforums.codeplex.com/workitem/1583	 */
/*														 */
/*********************************************************/

/*******************************************************************/
/*
	Instructions for use at 
	http://activeforums.codeplex.com/wikipage?title=Migrating%20from%20DNN%20Forums%205.0.3%20to%20Active%20Forums%205.0&referringTitle=Documentation
*/
/*******************************************************************/

/*
Create tables for storing old/new IDs to map for URL handling 
*/


if exists (select * from dbo.sysobjects where id = object_id(N'{databaseOwner}[{objectQualifier}dnntoaf_groups]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE {databaseOwner}[{objectQualifier}dnntoaf_groups]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'{databaseOwner}[{objectQualifier}dnntoaf_topics]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE {databaseOwner}[{objectQualifier}dnntoaf_topics]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'{databaseOwner}[{objectQualifier}dnntoaf_forums]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE {databaseOwner}[{objectQualifier}dnntoaf_forums]
GO

if exists (select * from dbo.sysobjects where id = object_id(N'{databaseOwner}[{objectQualifier}dnntoaf_replies]') and OBJECTPROPERTY(id, N'IsUserTable') = 1)
DROP TABLE {databaseOwner}[{objectQualifier}dnntoaf_replies]
GO



CREATE TABLE {databaseOwner}[{objectQualifier}dnntoaf_groups]
	(
	oldgroupid int NOT NULL,
	newgroupid int NOT NULL
	)  
GO

ALTER TABLE {databaseOwner}[{objectQualifier}dnntoaf_groups] ADD CONSTRAINT
	PK_{objectQualifier}dnntoaf_groups PRIMARY KEY CLUSTERED 
	(
	newgroupid
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) 

GO



CREATE TABLE {databaseOwner}{objectQualifier}dnntoaf_forums
	(
	oldforumid int NOT NULL,
	newforumid int NOT NULL
	)  
GO


ALTER TABLE {databaseOwner}{objectQualifier}dnntoaf_forums ADD CONSTRAINT
	PK_{objectQualifier}conversion_dnntoaf_forums PRIMARY KEY CLUSTERED 
	(
	newforumid
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

CREATE TABLE {databaseOwner}{objectQualifier}dnntoaf_topics
	(
	oldpostid int NOT NULL,
	newtopicid int NOT NULL
	)  
GO

ALTER TABLE {databaseOwner}{objectQualifier}dnntoaf_topics ADD CONSTRAINT
	PK_{objectQualifier}dnntoaf_topics PRIMARY KEY CLUSTERED 
	(
	oldpostid,
	newtopicid
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO

CREATE TABLE {databaseOwner}{objectQualifier}dnntoaf_replies
	(
	oldpostid int NOT NULL,
	newreplyid int NOT NULL
	)  
GO

ALTER TABLE {databaseOwner}{objectQualifier}dnntoaf_replies ADD CONSTRAINT
	PK_{objectQualifier}dnntoaf_replies PRIMARY KEY CLUSTERED 
	(
	oldpostid,
	newreplyid
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

GO



/* stored procedure for configuring the default group settings */

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{databaseOwner}[{objectQualifier}activeforums_DefaultGroupSettings]') AND type in (N'P', N'PC'))
DROP PROCEDURE {databaseOwner}[{objectQualifier}activeforums_DefaultGroupSettings]
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}activeforums_DefaultGroupSettings]
@ModuleId int,
@GroupId int
AS
DECLARE @GroupKey nvarchar(150)
SET @GroupKey = 'G:' + CAST(@GroupId as nvarchar(100))

INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ALLOWATTACH','true')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ALLOWEMOTICONS','true')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ALLOWHTML','false')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ALLOWPOSTICON','true')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ALLOWRSS','true')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ALLOWSCRIPT','false')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ATTACHCOUNT','3')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ATTACHMAXHEIGHT','400')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ATTACHMAXSIZE','1000')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ATTACHMAXWIDTH','400')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ATTACHSTORE','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ATTACHTYPEALLOWED','.jpg,.png,.gif,.zip')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ATTACHUNIQUEFILENAMES','false')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'AUTOTRUSTLEVEL','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'DEFAULTTRUSTLEVEL','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'EDITORHEIGHT','350')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'EDITORSTYLE','2')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'EDITORTOOLBAR','bold,italic,underline,quote')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'EDITORTYPE','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'EDITORWIDTH','99%')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'EMAILADDRESS','')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'INDEXCONTENT','false')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'ISMODERATED','false')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'MODAPPROVETEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'MODDELETETEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'MODMOVETEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'MODNOTIFYTEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'MODREJECTTEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'PROFILETEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'QUICKREPLYFORMID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'REPLYFORMID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'TOPICFORMID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'TOPICSTEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'TOPICTEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @GroupKey, 'USEFILTER','true')

UPDATE {databaseOwner}[{objectQualifier}activeforums_Groups] SET GroupSettingsKey = @GroupKey WHERE ForumGroupId = @GroupId

GO

/* stored procedure for configuring the default forum settings */

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{databaseOwner}[{objectQualifier}activeforums_DefaultForumSettings]') AND type in (N'P', N'PC'))
DROP PROCEDURE {databaseOwner}[{objectQualifier}activeforums_DefaultForumSettings]
GO

CREATE PROCEDURE {databaseOwner}[{objectQualifier}activeforums_DefaultForumSettings]
@ModuleId int,
@ForumId int
AS
DECLARE @ForumKey nvarchar(150)
SET @ForumKey = 'F:' + CAST(@ForumId as nvarchar(100))

INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ALLOWATTACH','true')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ALLOWEMOTICONS','true')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ALLOWHTML','true')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ALLOWPOSTICON','true')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ALLOWRSS','true')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ALLOWSCRIPT','false')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ATTACHCOUNT','3')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ATTACHMAXHEIGHT','400')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ATTACHMAXSIZE','1000')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ATTACHMAXWIDTH','400')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ATTACHSTORE','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ATTACHTYPEALLOWED','.jpg,.png,.gif,.zip')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ATTACHUNIQUEFILENAMES','false')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'AUTOTRUSTLEVEL','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'DEFAULTTRUSTLEVEL','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'EDITORHEIGHT','350')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'EDITORSTYLE','2')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'EDITORTOOLBAR','bold,italic,underline,quote')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'EDITORTYPE','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'EDITORWIDTH','99%')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'EMAILADDRESS','')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'INDEXCONTENT','false')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'ISMODERATED','false')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'MODAPPROVETEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'MODDELETETEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'MODMOVETEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'MODNOTIFYTEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'MODREJECTTEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'PROFILETEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'QUICKREPLYFORMID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'REPLYFORMID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'TOPICFORMID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'TOPICSTEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'TOPICTEMPLATEID','0')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'USEFILTER','true')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'AUTOSUBSCRIBEENABLED','false')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'AUTOSUBSCRIBENEWTOPICSONLY','false')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'AUTOSUBSCRIBEROLES','')
INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Settings] (ModuleId, GroupKey, SettingName, SettingValue) VALUES (@ModuleId, @ForumKey, 'EDITORPERMITTEDUSERS','0')

UPDATE {databaseOwner}[{objectQualifier}activeforums_Forums] SET ForumSettingsKey = @ForumKey WHERE ForumId = @ForumId

GO


/*Drop any constraints that may cause insert problems */

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'{databaseOwner}[{objectQualifier}FK_activeforums_ForumTopics_{objectQualifier}activeforums_Forums]') AND parent_object_id = OBJECT_ID(N'{databaseOwner}[{objectQualifier}activeforums_ForumTopics]'))
ALTER TABLE {databaseOwner}[{objectQualifier}activeforums_ForumTopics] DROP CONSTRAINT [FK_{objectQualifier}activeforums_ForumTopics_{objectQualifier}activeforums_Forums]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'{databaseOwner}[{objectQualifier}FK_activeforums_ForumTopics_{objectQualifier}activeforums_Topics]') AND parent_object_id = OBJECT_ID(N'{databaseOwner}[{objectQualifier}activeforums_ForumTopics]'))
ALTER TABLE {databaseOwner}[{objectQualifier}activeforums_ForumTopics] DROP CONSTRAINT [FK_{objectQualifier}activeforums_ForumTopics_{objectQualifier}activeforums_Topics]
GO
IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'{databaseOwner}[{objectQualifier}FK_activeforums_Topics_{objectQualifier}activeforums_Content]') AND parent_object_id = OBJECT_ID(N'{databaseOwner}{objectQualifier}[activeforums_Topics]'))
ALTER TABLE {databaseOwner}[{objectQualifier}activeforums_Topics] DROP CONSTRAINT [FK_{objectQualifier}activeforums_Topics_{objectQualifier}activeforums_Content]
GO
/*End Constraint drops*/

SET NOCOUNT ON

/*Variables*/
DECLARE @TargetModuleId INT
SET @TargetModuleId = CHANGEME /* the Module ID of your Active Forum module */
DECLARE @SourceModuleId INT
SET @SourceModuleId = CHANGEME  /* the Module ID of your DNN Forum module */
DECLARE @PortalId INT
SET @PortalId = 0

/*Begin Row deletions for Groups and Forums*/

DELETE X 
	FROM {databaseOwner}[{objectQualifier}activeforums_Permissions] X
	JOIN {databaseOwner}[{objectQualifier}activeforums_Forums] as f on x.PermissionsId = f.PermissionsId
	JOIN {databaseOwner}[{objectQualifier}activeforums_Groups] as g on g.ForumGroupId = f.ForumGroupId AND g.ModuleId = @TargetModuleId

DELETE X 
	FROM {databaseOwner}[{objectQualifier}activeforums_Settings] X
	JOIN {databaseOwner}[{objectQualifier}activeforums_Forums] as f on x.GroupKey = f.ForumSettingsKey
	JOIN {databaseOwner}[{objectQualifier}activeforums_Groups] as g on g.ForumGroupId = f.ForumGroupId AND g.ModuleId = @TargetModuleId

DELETE X 
	FROM {databaseOwner}[{objectQualifier}activeforums_Permissions] X
	JOIN {databaseOwner}[{objectQualifier}activeforums_Groups] as g on g.PermissionsId = x.PermissionsId AND g.ModuleId = @TargetModuleId

DELETE X 
	FROM {databaseOwner}[{objectQualifier}activeforums_Settings] X
	JOIN {databaseOwner}[{objectQualifier}activeforums_Groups] as g on g.GroupSettingsKey = x.GroupKey AND g.ModuleId = @TargetModuleId

DELETE X 
	FROM {databaseOwner}[{objectQualifier}activeforums_Forums] X
	JOIN {databaseOwner}[{objectQualifier}activeforums_Settings] as s on s.GroupKey = x.ForumSettingsKey
	JOIN {databaseOwner}[{objectQualifier}activeforums_Groups] as g on g.ForumGroupId = x.ForumGroupId AND g.ModuleId = @TargetModuleId

DELETE X 
	FROM {databaseOwner}[{objectQualifier}activeforums_Forums] X
	JOIN {databaseOwner}[{objectQualifier}activeforums_Groups] as g on g.ForumGroupId = x.ForumGroupId AND g.ModuleId = @TargetModuleId
DELETE 
	FROM {databaseOwner}[{objectQualifier}activeforums_Groups] WHERE ModuleId = @TargetModuleId


/*End Row deletions for Groups and Forums*/


/*Begin Forum Groups Migration*/

SET IDENTITY_INSERT {databaseOwner}{objectQualifier}activeforums_Groups ON

INSERT INTO {databaseOwner}{objectQualifier}activeforums_Groups 
	(ForumGroupId, ModuleId, GroupName, SortOrder, Active, Hidden, GroupSettingsKey, GroupSecurityKey, PermissionsId)
	SELECT GroupId, @TargetModuleId, Name, SortOrder, 1, 0, '','',-1 FROM {databaseOwner}[{objectQualifier}Forum_Groups] WHERE ModuleID = @SourceModuleId
SET IDENTITY_INSERT {databaseOwner}[{objectQualifier}activeforums_Groups] OFF

/* populate the old/new group id table for mapping URLS */

INSERT INTO {databaseOwner}{objectQualifier}dnntoaf_groups 
	(oldgroupid, newgroupid)
	SELECT 
		fg.GroupId --oldid
		, afg.ForumGroupId
		FROM {databaseOwner}[{objectQualifier}Forum_Groups] fg
		join {databaseOwner}{objectQualifier}activeforums_Groups afg on (fg.Name = afg.GroupName)
		WHERE fg.ModuleID = @SourceModuleId



DECLARE @GroupId int
DECLARE group_cursor CURSOR FOR 
	SELECT ForumGroupId as GroupId
	FROM {databaseOwner}[{objectQualifier}activeforums_Groups] 
	WHERE ModuleID = @TargetModuleId;
OPEN group_cursor;
	FETCH NEXT FROM group_cursor 
	INTO @GroupId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--print @GroupId
		exec {databaseOwner}[{objectQualifier}activeforums_DefaultGroupSettings] @TargetModuleId, @GroupId;

		
	FETCH NEXT FROM group_cursor 
		INTO @GroupId;
	END
	CLOSE group_cursor;
	DEALLOCATE group_cursor;
/*End Forum Groups Migration */

 
/*Begin Forum Migration */

SET IDENTITY_INSERT {databaseOwner}[{objectQualifier}activeforums_Forums] ON

INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Forums]
	(ForumId, PortalId, ModuleId, ForumGroupId, ParentForumId, ForumName, ForumDesc, SortOrder, Active, Hidden, 
		TotalTopics, TotalReplies, LastPostId, ForumSettingsKey, ForumSecurityKey, DateCreated, DateUpdated,
		LastTopicId, LastReplyId, LastPostSubject, LastPostAuthorName, LastPostAuthorId, LastPostDate,
		PermissionsId, PrefixURL)
	SELECT f.ForumID, @PortalId, @TargetModuleId, f.GroupID, f.ParentID, f.Name, f.Description, f.SortOrder, f.IsActive, 0,
		f.TotalThreads, f.TotalPosts, -1, '','',f.CreatedDate, f.UpdatedDate, 
		-1, -1, '', '', -1, NULL, 
		-1, ''  from {databaseOwner}[{objectQualifier}Forum_Forums] as f INNER JOIN
			 {databaseOwner}[{objectQualifier}Forum_Groups] as g ON g.GroupID = f.GroupID
		WHERE g.ModuleID = @SourceModuleId
SET IDENTITY_INSERT {databaseOwner}[{objectQualifier}activeforums_Forums] OFF


INSERT INTO {databaseOwner}{objectQualifier}dnntoaf_forums
	(oldforumid, newforumid)
	SELECT 
		f.ForumId
		, af.ForumId
		FROM {databaseOwner}[{objectQualifier}Forum_Forums] f
		join {databaseOwner}{objectQualifier}activeforums_Forums af on (f.Name = af.ForumName and f.ParentID = af.ParentForumId)
		WHERE ModuleID = @TargetModuleId



/* cjh - we need to setup default FORUM settings otherwise nothing works */

/* activeforums_DefaultForumSettings */

DECLARE @cForumId int
DECLARE forum_cursor CURSOR FOR 
	SELECT ForumId as ForumId
	FROM {databaseOwner}[{objectQualifier}activeforums_Forums]
	WHERE ModuleID = @TargetModuleId;
OPEN forum_cursor;
	FETCH NEXT FROM forum_cursor 
	INTO @cForumId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--print @cForumId
		exec {databaseOwner}[{objectQualifier}activeforums_DefaultForumSettings] @TargetModuleId, @cForumId;
	FETCH NEXT FROM forum_cursor 
		INTO @cForumId;
	END
	CLOSE Forum_cursor;
	DEALLOCATE Forum_cursor;

/*End Forum Migration*/


/*Begin Row deletions for Topics and Replies*/

DELETE X 
	FROM {databaseOwner}[{objectQualifier}activeforums_Content] X
	JOIN {databaseOwner}[{objectQualifier}activeforums_Replies] as r on x.ContentId = r.ContentId
	JOIN {databaseOwner}[{objectQualifier}activeforums_ForumTopics] as ft ON r.TopicId = ft.TopicId 
	JOIN {databaseOwner}[{objectQualifier}activeforums_Forums] as f on ft.ForumId = f.ForumId AND f.ModuleId=@TargetModuleId

DELETE X 
	FROM {databaseOwner}[{objectQualifier}activeforums_Replies] X
	JOIN {databaseOwner}[{objectQualifier}activeforums_ForumTopics] as ft ON x.TopicId = ft.TopicId 
	JOIN {databaseOwner}[{objectQualifier}activeforums_Forums] as f on ft.ForumId = f.ForumId AND f.ModuleId=@TargetModuleId


DELETE X 
	FROM {databaseOwner}[{objectQualifier}activeforums_Content] X
	JOIN {databaseOwner}[{objectQualifier}activeforums_Topics] as t on x.ContentId = t.ContentId
	JOIN {databaseOwner}[{objectQualifier}activeforums_ForumTopics] as ft ON t.TopicId = ft.TopicId 
	JOIN {databaseOwner}[{objectQualifier}activeforums_Forums] as f on ft.ForumId = f.ForumId AND f.ModuleId=@TargetModuleId


DELETE X 
	FROM {databaseOwner}[{objectQualifier}activeforums_Topics] X
	JOIN {databaseOwner}[{objectQualifier}activeforums_ForumTopics] as ft ON x.TopicId = ft.TopicId 
	JOIN {databaseOwner}[{objectQualifier}activeforums_Forums] as f on ft.ForumId = f.ForumId AND f.ModuleId=@TargetModuleId

DELETE X 
	FROM {databaseOwner}[{objectQualifier}activeforums_ForumTopics] as X 
	JOIN {databaseOwner}[{objectQualifier}activeforums_Forums] as f on f.ForumId = x.ForumId AND f.ModuleId=@TargetModuleId	
	
/*End Row deletions*/


/*Begin Topic Migration*/


DECLARE @PostId int, @UserId int, @RemoteAddr nvarchar(150), @Subject nvarchar(1000), @Body nvarchar(max), @CreatedDate datetime, @UpdatedDate datetime,
	@IsApproved bit, @IsLocked bit, @IsClosed bit, @DateApproved datetime, @PostReported bit, @Addressed bit, @ParseInfo bit,
	@ForumId int, @IsPinned bit, @Views int, @TopicStatus int, @ContentItemId int, @ThreadId int
DECLARE topic_cursor CURSOR FOR 
	SELECT p.PostId, p.UserId, p.RemoteAddr, p.Subject, p.Body, p.CreatedDate, p.UpdatedDate, p.IsApproved, 
		p.IsLocked, p.IsClosed, p.DateApproved, p.PostReported, p.Addressed, p.ParseInfo,
		t.ForumID, t.IsPinned, t.[Views], t.ThreadStatus, t.ContentItemID, t.ThreadID
	FROM {databaseOwner}[{objectQualifier}Forum_Posts] as p INNER JOIN
		{databaseOwner}[{objectQualifier}Forum_Threads] as t ON t.ThreadID = p.ThreadID INNER JOIN
		{databaseOwner}[{objectQualifier}Forum_Forums] as f ON t.ForumID = f.ForumID INNER JOIN
		{databaseOwner}[{objectQualifier}Forum_Groups] as g on f.GroupID = g.GroupID
	WHERE p.ParentPostID = 0 AND g.ModuleID = @SourceModuleId;
	
	OPEN topic_cursor;
	FETCH NEXT FROM topic_cursor 
	INTO @PostId, @UserId, @RemoteAddr, @Subject, @Body, @CreatedDate, @UpdatedDate,
	@IsApproved, @IsLocked, @IsClosed, @DateApproved, @PostReported, @Addressed, @ParseInfo,
	@ForumId, @IsPinned, @Views, @TopicStatus, @ContentItemId, @ThreadId;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @ContentId int
		INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Content] 
			(Subject, Summary, Body, DateCreated, DateUpdated, AuthorId, AuthorName, IsDeleted, IPAddress, ContentItemId)
			VALUES
			(@Subject, NULL, @Body, @CreatedDate, @UpdatedDate, @UserId, NULL, 0, @RemoteAddr, @ContentItemId)
		SET @ContentId = SCOPE_IDENTITY();
		SET IDENTITY_INSERT {databaseOwner}{objectQualifier}activeforums_Topics ON
		INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Topics]
			(TopicId, ContentId, ViewCount, ReplyCount, IsLocked, IsPinned, TopicIcon, StatusId, IsApproved, IsRejected, IsAnnounce, IsArchived, TopicType, [Priority])
			VALUES
			(@ThreadId, @ContentId, @Views, 0, @IsLocked, @IsPinned, NULL, @TopicStatus, @IsApproved, 0, 0, 0, 0, 0)
		SET IDENTITY_INSERT {databaseOwner}{objectQualifier}activeforums_Topics OFF

		INSERT INTO {databaseOwner}[{objectQualifier}activeforums_ForumTopics] 
			(ForumId, TopicId) VALUES (@ForumId, @ThreadId)

		/* cjh Adding the ThreadID mapping for URL rewriting */
		INSERT INTO {databaseOwner}[{objectQualifier}dnntoaf_topics]
			(oldpostid, newTopicId) VALUES (@ThreadId,@ThreadId)

		FETCH NEXT FROM topic_cursor 
		INTO @PostId, @UserId, @RemoteAddr, @Subject, @Body, @CreatedDate, @UpdatedDate,
			@IsApproved, @IsLocked, @IsClosed, @DateApproved, @PostReported, @Addressed, @ParseInfo,
			@ForumId, @IsPinned, @Views, @TopicStatus, @ContentItemId, @ThreadId;
	END
	CLOSE topic_cursor;
	DEALLOCATE topic_cursor;
/*End Topic Migration */

/*Begin Reply Migration */

DECLARE reply_cursor CURSOR FOR 
	SELECT p.PostId, p.UserId, p.RemoteAddr, p.Subject, p.Body, p.CreatedDate, p.UpdatedDate, p.IsApproved, 
		p.IsLocked, p.IsClosed, p.DateApproved, p.PostReported, p.Addressed, p.ParseInfo,
		t.ForumID, t.IsPinned, t.[Views], t.ThreadStatus, t.ContentItemID, t.ThreadID
	FROM {databaseOwner}[{objectQualifier}Forum_Posts] as p INNER JOIN
		{databaseOwner}[{objectQualifier}Forum_Threads] as t ON t.ThreadID = p.ThreadID INNER JOIN
		{databaseOwner}[{objectQualifier}Forum_Forums] as f ON t.ForumID = f.ForumID INNER JOIN
		{databaseOwner}[{objectQualifier}Forum_Groups] as g on f.GroupID = g.GroupID
	WHERE p.ParentPostID > 0 AND g.ModuleID = @SourceModuleId;
	
	OPEN reply_cursor;
	FETCH NEXT FROM reply_cursor 
	INTO @PostId, @UserId, @RemoteAddr, @Subject, @Body, @CreatedDate, @UpdatedDate,
	@IsApproved, @IsLocked, @IsClosed, @DateApproved, @PostReported, @Addressed, @ParseInfo,
	@ForumId, @IsPinned, @Views, @TopicStatus, @ContentItemId, @ThreadId;
	WHILE @@FETCH_STATUS = 0
	BEGIN
		DECLARE @ReplyContentId int;
		INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Content] 
			(Subject, Summary, Body, DateCreated, DateUpdated, AuthorId, AuthorName, IsDeleted, IPAddress, ContentItemId)
			VALUES
			(@Subject, NULL, @Body, @CreatedDate, @UpdatedDate, @UserId, NULL, 0, @RemoteAddr, @ContentItemId)
		SET @ReplyContentId = SCOPE_IDENTITY();

		DECLARE @NewReplyId int
		INSERT INTO {databaseOwner}[{objectQualifier}activeforums_Replies]
			(TopicId, ReplyToId, ContentId, IsApproved, IsRejected, StatusId, IsDeleted)
			VALUES
			(@ThreadId, NULL, @ReplyContentId, @IsApproved, 0, @TopicStatus, 0)
		Set @NewReplyId = SCOPE_IDENTITY();

		/* cjh Adding the ThreadID mapping for URL rewriting */
		INSERT INTO {databaseOwner}[{objectQualifier}dnntoaf_replies]
			(oldpostid, newReplyId) VALUES (@PostId,@NewReplyId)

		FETCH NEXT FROM reply_cursor 
		INTO @PostId, @UserId, @RemoteAddr, @Subject, @Body, @CreatedDate, @UpdatedDate,
			@IsApproved, @IsLocked, @IsClosed, @DateApproved, @PostReported, @Addressed, @ParseInfo,
			@ForumId, @IsPinned, @Views, @TopicStatus, @ContentItemId, @ThreadId;
	END
	CLOSE reply_cursor;
	DEALLOCATE reply_cursor;

/* update the replytoid for all replies */

declare @RepPostId int --old post id
declare @NewReplyToId int --new reply to id

/* sets reply ID for anything that points to a topic */

declare reply_to_cursor CURSOR FOR
	select 
		fp.PostID
		, (select r.newReplyId from {databaseOwner}[{objectQualifier}dnntoaf_replies] r where r.oldpostid = fp.postid) 
		, (select t2.newtopicid from {databaseOwner}[{objectQualifier}dnntoaf_topics] t2 where t2.oldpostid = fp.ParentPostID)
	from 
		{databaseOwner}[{objectQualifier}forum_posts] fp
	where 
		(select t2.newtopicid from {databaseOwner}[{objectQualifier}dnntoaf_topics] t2 where t2.oldpostid = fp.ParentPostID) >0

	OPEN reply_to_cursor;
	FETCH NEXT FROM reply_to_cursor
	into @RepPostId, @NewReplyId, @NewReplyToId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		update {databaseOwner}[{objectQualifier}activeforums_Replies]
			set ReplyToId = @NewReplyToId
			where ReplyId = @NewReplyId		
			
	FETCH NEXT FROM reply_to_cursor
	into @RepPostId, @NewReplyId, @NewReplyToId

	END
	CLOSE reply_to_cursor;
	DEALLOCATE reply_to_cursor;

/* sets reply ID for anything that points to a reply */

declare reply_to_cursor CURSOR FOR
	select 
	fp.PostID as 'Old Post Id'
	, (select r.newReplyId from {databaseOwner}[{objectQualifier}dnntoaf_replies] r where r.oldpostid = fp.postid) 'newreplyid'
	, (select r2.newReplyId from {databaseOwner}[{objectQualifier}dnntoaf_replies] r2 where r2.oldpostid = fp.ParentPostID) 'replytoid'
	
	from {databaseOwner}[{objectQualifier}forum_posts] fp
	where 

	(select r2.newReplyId from {databaseOwner}[{objectQualifier}dnntoaf_replies] r2 where r2.oldpostid = fp.ParentPostID) >0
	
	OPEN reply_to_cursor;
	FETCH NEXT FROM reply_to_cursor
	into @RepPostId, @NewReplyId, @NewReplyToId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		update {databaseOwner}[{objectQualifier}activeforums_Replies]
			set ReplyToId = @NewReplyToId
			where ReplyId = @NewReplyId		
			
	FETCH NEXT FROM reply_to_cursor
	into @RepPostId, @NewReplyId, @NewReplyToId

	END
	CLOSE reply_to_cursor;
	DEALLOCATE reply_to_cursor;



/*End Reply Migration*/



/* ----------------- BEGIN Permission Migration ----------------------------- */
DECLARE @RegUsersRoleID INT
SET @RegUsersRoleID = ( SELECT  RoleID
                        FROM    {databaseOwner}{objectQualifier}Roles
                        WHERE   RoleName = 'Registered Users'
                                AND PortalID = @PortalID
                      )

/* Retrieve Module level Permissions */ 
DECLARE @GlobalModRoles NVARCHAR(490)
DECLARE @GlobalModUsers NVARCHAR(490)
SET @GlobalModRoles = ISNULL(( SELECT DISTINCT
                                        CONVERT(NVARCHAR(10), ( RoleID ))
                                        + ';' AS 'data()'
                               FROM     {databaseOwner}{objectQualifier}ModulePermission MP
                                        INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                               WHERE    ModuleID = @SourceModuleId
                                        AND RoleID > -1
                                        AND ( PermissionKey = 'FORUMADMIN'
                                              OR PermissionKey = 'FORUMGLBMOD'
                                            )
                             FOR
                               XML PATH('')
                             ), '')
SET @GlobalModUsers = ISNULL(( SELECT DISTINCT
                                        CONVERT(NVARCHAR(10), ( UserID ))
                                        + ';' AS 'data()'
                               FROM     {databaseOwner}{objectQualifier}ModulePermission MP
                                        INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                               WHERE    ModuleID = @SourceModuleId
                                        AND UserID > 0
                                        AND ( PermissionKey = 'FORUMADMIN'
                                              OR PermissionKey = 'FORUMGLBMOD'
                                            )
                             FOR
                               XML PATH('')
                             ), '')
                                     
/* CJH - Commented out the @ForumID and @GroupID because they were declared earlier in the scripts */
DECLARE @ForumBehavior INT
/*
DECLARE @ForumID INT
DECLARE @GroupID INT
*/

/* Lets get this party started */
DECLARE Permission_cursor CURSOR
FOR
    SELECT  FF.ForumID ,
            ForumBehavior ,
            FF.GroupID
    FROM    {databaseOwner}{objectQualifier}Forum_Forums FF
            INNER JOIN {databaseOwner}{objectQualifier}Forum_Groups FG ON FF.GroupID = FG.GroupID
    WHERE   ModuleID = @SourceModuleId
    ORDER BY ForumBehavior ASC ,
            GroupID ASC ,
            PublicView ASC ,
            ForumID ASC

OPEN Permission_cursor
FETCH NEXT FROM Permission_cursor 
INTO @ForumID, @ForumBehavior, @GroupID
WHILE @@FETCH_STATUS = 0 
    BEGIN
        BEGIN TRANSACTION                      
		/* @CanView also covers @CanRead */
        DECLARE @CanViewUser NVARCHAR(490)
        DECLARE @CanViewRole NVARCHAR(490)
        DECLARE @CanView NVARCHAR(1000)
        DECLARE @CanCreateUser NVARCHAR(490)
        DECLARE @CanCreateRole NVARCHAR(490)
        DECLARE @CanCreate NVARCHAR(1000)
        DECLARE @CanReplyUser NVARCHAR(490)
        DECLARE @CanReplyRole NVARCHAR(490)
        DECLARE @CanReply NVARCHAR(1000)
        DECLARE @CanPinUser NVARCHAR(490)
        DECLARE @CanPinRole NVARCHAR(490)
        DECLARE @CanPin NVARCHAR(1000)
        DECLARE @CanLockUser NVARCHAR(490)
        DECLARE @CanLockRole NVARCHAR(490)
        DECLARE @CanLock NVARCHAR(1000)
		/* @CanModerate represents the AF columns: CanEdit, CanDelete, CanPoll, CanTrust, CanModApprove, CanModMove, CanModSplit, CanModDelete, CanModEdit */
        DECLARE @CanModerateUser NVARCHAR(490)
        DECLARE @CanModerateRole NVARCHAR(490)
        DECLARE @CanModerate NVARCHAR(1000)

        SET @CanViewRole = ISNULL(( SELECT DISTINCT
                                            CONVERT(NVARCHAR(10), ( RoleID ))
                                            + ';' AS 'data()'
                                    FROM    {databaseOwner}{objectQualifier}Forum_ForumPermission
                                    WHERE   ForumID = @ForumID
                                            AND RoleID > -1
                                            AND PermissionID = 1
                                            AND RoleID NOT IN (
                                            SELECT DISTINCT
                                                    ( RoleID )
                                            FROM    {databaseOwner}{objectQualifier}ModulePermission MP
                                                    INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                                            WHERE   ModuleID = @SourceModuleId
                                                    AND RoleID > -1
                                                    AND ( PermissionKey = 'FORUMADMIN'
                                                          OR PermissionKey = 'FORUMGLBMOD'
                                                        ) )
                                  FOR
                                    XML PATH('')
                                  ), '')	
        SET @CanViewUser = ISNULL(( SELECT DISTINCT
                                            CONVERT(NVARCHAR(10), ( UserID ))
                                            + ';' AS 'data()'
                                    FROM    {databaseOwner}{objectQualifier}Forum_ForumPermission
                                    WHERE   ForumID = @ForumID
                                            AND UserID > 0
                                            AND PermissionID = 1
                                            AND UserID NOT IN (
                                            SELECT DISTINCT
                                                    ( UserID )
                                            FROM    {databaseOwner}{objectQualifier}ModulePermission MP
                                                    INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                                            WHERE   ModuleID = @SourceModuleId
                                                    AND UserID > 0
                                                    AND ( PermissionKey = 'FORUMADMIN'
                                                          OR PermissionKey = 'FORUMGLBMOD'
                                                        ) )
                                  FOR
                                    XML PATH('')
                                  ), '')             
        SET @CanCreateRole = ISNULL(( SELECT DISTINCT
                                                CONVERT(NVARCHAR(10), ( RoleID ))
                                                + ';' AS 'data()'
                                      FROM      {databaseOwner}{objectQualifier}Forum_ForumPermission
                                      WHERE     ForumID = @ForumID
                                                AND RoleID > -1
                                                AND PermissionID = 2
                                                AND RoleID NOT IN (
                                                SELECT DISTINCT
                                                        ( RoleID )
                                                FROM    {databaseOwner}{objectQualifier}ModulePermission MP
                                                        INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                                                WHERE   ModuleID = @SourceModuleId
                                                        AND RoleID > -1
                                                        AND ( PermissionKey = 'FORUMADMIN'
                                                              OR PermissionKey = 'FORUMGLBMOD'
                                                            ) )
                                    FOR
                                      XML PATH('')
                                    ), '')               
        SET @CanCreateUser = ISNULL(( SELECT DISTINCT
                                                CONVERT(NVARCHAR(10), ( UserID ))
                                                + ';' AS 'data()'
                                      FROM      {databaseOwner}{objectQualifier}Forum_ForumPermission
                                      WHERE     ForumID = @ForumID
                                                AND UserID > 0
                                                AND PermissionID = 2
                                                AND UserID NOT IN (
                                                SELECT DISTINCT
                                                        ( UserID )
                                                FROM    {databaseOwner}{objectQualifier}ModulePermission MP
                                                        INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                                                WHERE   ModuleID = @SourceModuleId
                                                        AND UserID > 0
                                                        AND ( PermissionKey = 'FORUMADMIN'
                                                              OR PermissionKey = 'FORUMGLBMOD'
                                                            ) )
                                    FOR
                                      XML PATH('')
                                    ), '')                
        SET @CanReplyRole = ISNULL(( SELECT DISTINCT
                                            CONVERT(NVARCHAR(10), ( RoleID ))
                                            + ';' AS 'data()'
                                     FROM   {databaseOwner}{objectQualifier}Forum_ForumPermission
                                     WHERE  ForumID = @ForumID
                                            AND RoleID > -1
                                            AND PermissionID = 3
                                            AND RoleID NOT IN (
                                            SELECT DISTINCT
                                                    ( RoleID )
                                            FROM    {databaseOwner}{objectQualifier}ModulePermission MP
                                                    INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                                            WHERE   ModuleID = @SourceModuleId
                                                    AND RoleID > -1
                                                    AND ( PermissionKey = 'FORUMADMIN'
                                                          OR PermissionKey = 'FORUMGLBMOD'
                                                        ) )
                                   FOR
                                     XML PATH('')
                                   ), '')            
        SET @CanReplyUser = ISNULL(( SELECT DISTINCT
                                            CONVERT(NVARCHAR(10), ( UserID ))
                                            + ';' AS 'data()'
                                     FROM   {databaseOwner}{objectQualifier}Forum_ForumPermission
                                     WHERE  ForumID = @ForumID
                                            AND UserID > 0
                                            AND PermissionID = 3
                                            AND UserID NOT IN (
                                            SELECT DISTINCT
                                                    ( UserID )
                                            FROM    {databaseOwner}{objectQualifier}ModulePermission MP
                                                    INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                                            WHERE   ModuleID = @SourceModuleId
                                                    AND UserID > 0
                                                    AND ( PermissionKey = 'FORUMADMIN'
                                                          OR PermissionKey = 'FORUMGLBMOD'
                                                        ) )
                                   FOR
                                     XML PATH('')
                                   ), '')               
        SET @CanPinRole = ISNULL(( SELECT DISTINCT
                                            CONVERT(NVARCHAR(10), ( RoleID ))
                                            + ';' AS 'data()'
                                   FROM     {databaseOwner}{objectQualifier}Forum_ForumPermission
                                   WHERE    ForumID = @ForumID
                                            AND RoleID > -1
                                            AND PermissionID = 6
                                            AND RoleID NOT IN (
                                            SELECT DISTINCT
                                                    ( RoleID )
                                            FROM    {databaseOwner}{objectQualifier}ModulePermission MP
                                                    INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                                            WHERE   ModuleID = @SourceModuleId
                                                    AND RoleID > -1
                                                    AND ( PermissionKey = 'FORUMADMIN'
                                                          OR PermissionKey = 'FORUMGLBMOD'
                                                        ) )
                                 FOR
                                   XML PATH('')
                                 ), '')                 
        SET @CanPinUser = ISNULL(( SELECT DISTINCT
                                            CONVERT(NVARCHAR(10), ( UserID ))
                                            + ';' AS 'data()'
                                   FROM     {databaseOwner}{objectQualifier}Forum_ForumPermission
                                   WHERE    ForumID = @ForumID
                                            AND UserID > 0
                                            AND PermissionID = 6
                                            AND UserID NOT IN (
                                            SELECT DISTINCT
                                                    ( UserID )
                                            FROM    {databaseOwner}{objectQualifier}ModulePermission MP
                                                    INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                                            WHERE   ModuleID = @SourceModuleId
                                                    AND UserID > 0
                                                    AND ( PermissionKey = 'FORUMADMIN'
                                                          OR PermissionKey = 'FORUMGLBMOD'
                                                        ) )
                                 FOR
                                   XML PATH('')
                                 ), '')             
        SET @CanLockRole = ISNULL(( SELECT DISTINCT
                                            CONVERT(NVARCHAR(10), ( RoleID ))
                                            + ';' AS 'data()'
                                    FROM    {databaseOwner}{objectQualifier}Forum_ForumPermission
                                    WHERE   ForumID = @ForumID
                                            AND RoleID > -1
                                            AND PermissionID = 7
                                            AND RoleID NOT IN (
                                            SELECT DISTINCT
                                                    ( RoleID )
                                            FROM    {databaseOwner}{objectQualifier}ModulePermission MP
                                                    INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                                            WHERE   ModuleID = @SourceModuleId
                                                    AND RoleID > -1
                                                    AND ( PermissionKey = 'FORUMADMIN'
                                                          OR PermissionKey = 'FORUMGLBMOD'
                                                        ) )
                                  FOR
                                    XML PATH('')
                                  ), '')               
        SET @CanLockUser = ISNULL(( SELECT DISTINCT
                                            CONVERT(NVARCHAR(10), ( UserID ))
                                            + ';' AS 'data()'
                                    FROM    {databaseOwner}{objectQualifier}Forum_ForumPermission
                                    WHERE   ForumID = @ForumID
                                            AND UserID > 0
                                            AND PermissionID = 7
                                            AND UserID NOT IN (
                                            SELECT DISTINCT
                                                    ( UserID )
                                            FROM    {databaseOwner}{objectQualifier}ModulePermission MP
                                                    INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                                            WHERE   ModuleID = @SourceModuleId
                                                    AND UserID > 0
                                                    AND ( PermissionKey = 'FORUMADMIN'
                                                          OR PermissionKey = 'FORUMGLBMOD'
                                                        ) )
                                  FOR
                                    XML PATH('')
                                  ), '')	
        SET @CanModerateRole = ISNULL(( SELECT DISTINCT
                                                CONVERT(NVARCHAR(10), ( RoleID ))
                                                + ';' AS 'data()'
                                        FROM    {databaseOwner}{objectQualifier}Forum_ForumPermission
                                        WHERE   ForumID = @ForumID
                                                AND RoleID > -1
                                                AND PermissionID = 4
                                                AND RoleID NOT IN (
                                                SELECT DISTINCT
                                                        ( RoleID )
                                                FROM    {databaseOwner}{objectQualifier}ModulePermission MP
                                                        INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                                                WHERE   ModuleID = @SourceModuleId
                                                        AND RoleID > -1
                                                        AND ( PermissionKey = 'FORUMADMIN'
                                                              OR PermissionKey = 'FORUMGLBMOD'
                                                            ) )
                                      FOR
                                        XML PATH('')
                                      ), '')                                                    
        SET @CanModerateUser = ISNULL(( SELECT DISTINCT
                                                CONVERT(NVARCHAR(10), ( UserID ))
                                                + ';' AS 'data()'
                                        FROM    {databaseOwner}{objectQualifier}Forum_ForumPermission
                                        WHERE   ForumID = @ForumID
                                                AND UserID > 0
                                                AND PermissionID = 4
                                                AND UserID NOT IN (
                                                SELECT DISTINCT
                                                        ( UserID )
                                                FROM    {databaseOwner}{objectQualifier}ModulePermission MP
                                                        INNER JOIN {databaseOwner}{objectQualifier}Permission P ON MP.PermissionID = P.PermissionID
                                                WHERE   ModuleID = @SourceModuleId
                                                        AND UserID > 0
                                                        AND ( PermissionKey = 'FORUMADMIN'
                                                              OR PermissionKey = 'FORUMGLBMOD'
                                                            ) )
                                      FOR
                                        XML PATH('')
                                      ), '')              
/* -- Begin View -- */
        IF @ForumBehavior < 4 
            BEGIN
			-- add all users and anon roles
                SET @CanViewRole = @CanViewRole + '-3;-1;' 
            END
        SET @CanView = @CanViewRole + @GlobalModRoles + '|' + @CanViewUser
            + @GlobalModUsers + '||'
/* -- Begin Create -- */
        SET @CanCreateRole = @CanCreateRole + @CanModerateRole
        SET @CanCreateUser = @CanCreateUser + @CanModerateUser	
        IF @ForumBehavior = 0
            OR @ForumBehavior = 2 
            BEGIN
			-- add in registered users role for all public posting forums
                SET @CanCreateRole = @CanCreateRole
                    + CONVERT(NVARCHAR(10), @RegUsersRoleID) + ';'
            END				
	-- add admin/global mod here, close it up for use.
        SET @CanCreate = @CanCreateRole + @GlobalModRoles + '|'
            + @CanCreateUser + @GlobalModUsers + '||'
/* -- Begin Reply -- */
        SET @CanReplyRole = @CanReplyRole + @CanModerateRole
        SET @CanReplyUser = @CanReplyUser + @CanModerateUser	
        IF @ForumBehavior = 0
            OR @ForumBehavior = 2 
            BEGIN
			-- add in registered users role for all public posting forums
                SET @CanReplyRole = @CanReplyRole
                    + CONVERT(NVARCHAR(10), @RegUsersRoleID) + ';'
            END				
        SET @CanReply = @CanReplyRole + @GlobalModRoles + '|' + @CanReplyUser
            + @GlobalModUsers + '||'           
/* -- Begin Pin -- */
        SET @CanPin = @CanPinRole + @GlobalModRoles + '|' + @CanPinUser
            + @GlobalModUsers + '||'	          
/* -- Begin Lock -- */
        SET @CanLock = @CanLockRole + @GlobalModRoles + '|' + @CanLockUser
            + @GlobalModUsers + '||'	
/* -- Begin Moderator */
        SET @CanModerate = @CanModerateRole + @GlobalModRoles + '|'
            + @CanModerateUser + @GlobalModUsers + '||'
/* Determine if we have this set of permissions already in AF */	
        DECLARE @PermissionsId INT
        SET @PermissionsId = ISNULL(( SELECT    PermissionsId
                                      FROM      {databaseOwner}[{objectQualifier}activeforums_Permissions]
                                      WHERE     CanView = @CanView
                                                AND CanCreate = @CanCreate
                                                AND CanReply = @CanReply
                                                AND CanLock = @CanLock
                                                AND CanPin = @CanPin
                                                AND CanEdit = @CanModerate
                                    ), -1)
        IF @PermissionsId < 0 
            BEGIN
                INSERT  INTO {databaseOwner}[{objectQualifier}activeforums_Permissions]
                        ( CanView ,
                          CanRead ,
                          CanCreate ,
                          CanReply ,
                          CanEdit ,
                          CanDelete ,
                          CanLock ,
                          CanPin ,
                          CanAttach ,
                          CanPoll ,
                          CanBlock ,
                          CanTrust ,
                          CanSubscribe ,
                          CanAnnounce ,
                          CanModApprove ,
                          CanModMove ,
                          CanModSplit ,
                          CanModDelete ,
                          CanModUser ,
                          CanModEdit ,
                          CanModLock ,
                          CanModPin
			        
                        )
                VALUES  ( @CanView ,
                          @CanView ,
                          @CanCreate ,
                          @CanReply ,
                          @CanModerate ,
                          @CanModerate ,
                          @CanLock ,
                          @CanPin ,
                          @CanModerate ,
                          @CanModerate ,
                          N'' , -- CanBlock - nvarchar(1000)
                          @CanModerate ,
                          @CanModerate ,
                          @CanModerate ,
                          @CanModerate ,
                          @CanModerate ,
                          @CanModerate ,
                          @CanModerate ,
                          N'' , -- CanModUser - nvarchar(1000)
                          @CanModerate ,
                          @CanLock ,
                          @CanPin 
			        
                        )
                SET @PermissionsId = SCOPE_IDENTITY()		
            END		
            
/* Finally, lets assign us some permissions */
        UPDATE  {databaseOwner}[{objectQualifier}activeforums_Forums]
        SET     PermissionsId = @PermissionsId
        WHERE   ForumId = @ForumId

/* Handle Group Permission (for the forum) */
        DECLARE @TempGroupPermsId INT
        SET @TempGroupPermsId = ISNULL(( SELECT PermissionsId
                                         FROM   {databaseOwner}[{objectQualifier}activeforums_Groups]
                                         WHERE  ForumGroupId = @GroupID
                                       ), -1)
        IF @TempGroupPermsId < 1 
            BEGIN
			-- assign the permissions set to the group (our cursor query assures we always assign the least restrictive at the group level)
                UPDATE  {databaseOwner}[{objectQualifier}activeForums_Groups]
                SET     PermissionsId = @PermissionsId
                WHERE   ForumGroupId = @GroupID
            END

        COMMIT

        FETCH NEXT FROM Permission_cursor INTO @ForumID, @ForumBehavior,
            @GroupID
    END
CLOSE Permission_cursor
DEALLOCATE Permission_cursor




/* cjh - we also need to set the next/prev topic IDs on topics in a forum */

DECLARE @ncForumId int
DECLARE forum_cursor CURSOR FOR 
	SELECT ForumId as ForumId
	FROM {databaseOwner}[{objectQualifier}activeforums_Forums]
	WHERE ModuleID = @TargetModuleId;
OPEN forum_cursor;
	FETCH NEXT FROM forum_cursor 
	INTO @ncForumId
	WHILE @@FETCH_STATUS = 0
	BEGIN
		--print @ncForumId
		EXEC {databaseOwner}[{objectQualifier}activeforums_SaveTopicNextPrev] @ncForumId;
	FETCH NEXT FROM forum_cursor 
		INTO @ncForumId;
	END
	CLOSE Forum_cursor;
	DEALLOCATE Forum_cursor;



GO


/*Add back any constraints that were previously dropped*/
/* CJH - Moved because the GO's kill the declared parameters @portalid, etc */

ALTER TABLE {databaseOwner}[{objectQualifier}activeforums_ForumTopics]  WITH CHECK ADD  CONSTRAINT [FK_{objectQualifier}activeforums_ForumTopics_{objectQualifier}activeforums_Forums] FOREIGN KEY([ForumId])
REFERENCES {databaseOwner}[{objectQualifier}activeforums_Forums] ([ForumId])
GO
ALTER TABLE {databaseOwner}[{objectQualifier}activeforums_ForumTopics]  WITH NOCHECK ADD  CONSTRAINT [FK_{objectQualifier}activeforums_ForumTopics_{objectQualifier}activeforums_Topics] FOREIGN KEY([TopicId])
REFERENCES {databaseOwner}[{objectQualifier}activeforums_Topics] ([TopicId])
GO

ALTER TABLE {databaseOwner}[{objectQualifier}activeforums_ForumTopics] NOCHECK CONSTRAINT [FK_{objectQualifier}activeforums_ForumTopics_{objectQualifier}activeforums_Topics]
GO

ALTER TABLE {databaseOwner}[{objectQualifier}activeforums_Topics]  WITH NOCHECK ADD  CONSTRAINT [FK_{objectQualifier}activeforums_Topics_{objectQualifier}activeforums_Content] FOREIGN KEY([ContentId])
REFERENCES {databaseOwner}[{objectQualifier}activeforums_Content] ([ContentId])
GO

ALTER TABLE {databaseOwner}[{objectQualifier}activeforums_Topics] NOCHECK CONSTRAINT [FK_{objectQualifier}activeforums_Topics_{objectQualifier}activeforums_Content]
GO



/*update the ReplyCount field of Topics */
UPDATE 
	{databaseOwner}[{objectQualifier}activeforums_Topics]
SET 
	ReplyCount = (SELECT COUNT(*) from {databaseOwner}[{objectQualifier}activeforums_Replies] WHERE {databaseOwner}{objectQualifier}activeforums_Replies.TopicId = {databaseOwner}{objectQualifier}activeforums_Topics.TopicId  )
GO

/* Update LastPostAuthorName of Content */
UPDATE 
	{databaseOwner}[{objectQualifier}activeforums_Content]
SET 
	AuthorName = 
	(
		SELECT 
			Displayname
		FROM	
			Users			
		
		WHERE {databaseOwner}{objectQualifier}activeforums_Content.AuthorId = Users.UserID 		
	 )
GO


/* update the LastReplyId field of ForumTopics */
/* cjh - modified the logic for LastReplyId because of performance issues, also data appeared to be incorrect */
UPDATE 
	{databaseOwner}[{objectQualifier}activeforums_ForumTopics]
SET 
	LastReplyId =
	(
		SELECT 
			max(ReplyID)
		FROM	
			{databaseOwner}[{objectQualifier}activeforums_Replies]
				
		WHERE 
		{databaseOwner}[{objectQualifier}activeforums_Replies].TopicId = {databaseOwner}{objectQualifier}activeforums_ForumTopics.TopicId 
	)
GO



/* Update LastTopicID of Forums */
UPDATE 
	{databaseOwner}[{objectQualifier}activeforums_Forums]
SET 
	LastTopicID = 
	isnull((
		SELECT 
			max({databaseOwner}{objectQualifier}activeforums_Topics.TopicID)
		FROM	
			{databaseOwner}[{objectQualifier}activeforums_Content] INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_Topics] ON {databaseOwner}{objectQualifier}activeforums_Content.ContentId = {databaseOwner}{objectQualifier}activeforums_Topics.ContentId INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_ForumTopics] ON {databaseOwner}{objectQualifier}activeforums_Topics.TopicId = {databaseOwner}{objectQualifier}activeforums_ForumTopics.TopicId
		
		WHERE {databaseOwner}{objectQualifier}activeforums_Content.DateUpdated = 
		(
			SELECT     
				MAX({databaseOwner}{objectQualifier}activeforums_Content.DateUpdated) AS LastDate
			FROM         
				{databaseOwner}[{objectQualifier}activeforums_Content] INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_Topics] ON {databaseOwner}{objectQualifier}activeforums_Content.ContentId = {databaseOwner}{objectQualifier}activeforums_Topics.ContentId INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_ForumTopics] ON {databaseOwner}{objectQualifier}activeforums_Topics.TopicId = {databaseOwner}{objectQualifier}activeforums_ForumTopics.TopicId
			WHERE 
				{databaseOwner}{objectQualifier}activeforums_ForumTopics.ForumId = {databaseOwner}{objectQualifier}activeforums_Forums.ForumId 
		)			
	 ),-1)
GO


/* Update LastReplyID of Forums */
UPDATE 
	{databaseOwner}[{objectQualifier}activeforums_Forums]
SET 
	LastReplyID = 
	isnull((
		SELECT 
			max(ReplyID)
		FROM	
			{databaseOwner}[{objectQualifier}activeforums_Content] INNER JOIN
            {databaseOwner}[{objectQualifier}activeforums_Replies] ON {databaseOwner}{objectQualifier}activeforums_Content.ContentId = {databaseOwner}{objectQualifier}activeforums_Replies.ContentId INNER JOIN
            {databaseOwner}[{objectQualifier}activeforums_Topics] ON {databaseOwner}{objectQualifier}activeforums_Replies.TopicId = {databaseOwner}{objectQualifier}activeforums_Topics.TopicId INNER JOIN
            {databaseOwner}[{objectQualifier}activeforums_ForumTopics] ON {databaseOwner}{objectQualifier}activeforums_Topics.TopicId = {databaseOwner}{objectQualifier}activeforums_ForumTopics.TopicId
		
		WHERE {databaseOwner}{objectQualifier}activeforums_Content.DateUpdated = 
		(
			SELECT     
				MAX({databaseOwner}{objectQualifier}activeforums_Content.DateUpdated) AS LastDate
			FROM         
				{databaseOwner}[{objectQualifier}activeforums_Content] INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_Replies] ON {databaseOwner}{objectQualifier}activeforums_Content.ContentId = {databaseOwner}{objectQualifier}activeforums_Replies.ContentId INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_Topics] ON {databaseOwner}{objectQualifier}activeforums_Replies.TopicId = {databaseOwner}{objectQualifier}activeforums_Topics.TopicId INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_ForumTopics] ON {databaseOwner}{objectQualifier}activeforums_Topics.TopicId = {databaseOwner}{objectQualifier}activeforums_ForumTopics.TopicId
			WHERE 
				{databaseOwner}{objectQualifier}activeforums_ForumTopics.ForumId = {databaseOwner}{objectQualifier}activeforums_Forums.ForumId 
		)
			
	 ),-1)
GO

/* Update LastPostSubject of Forums */
UPDATE 
	{databaseOwner}[{objectQualifier}activeforums_Forums]
SET 
	LastPostSubject = 
	(
		SELECT 
			max({databaseOwner}{objectQualifier}activeforums_Content.Subject)
		FROM	
			{databaseOwner}[{objectQualifier}activeforums_Content] INNER JOIN
            {databaseOwner}[{objectQualifier}activeforums_Replies] ON {databaseOwner}{objectQualifier}activeforums_Content.ContentId = {databaseOwner}{objectQualifier}activeforums_Replies.ContentId INNER JOIN
            {databaseOwner}[{objectQualifier}activeforums_Topics] ON {databaseOwner}{objectQualifier}activeforums_Replies.TopicId = {databaseOwner}{objectQualifier}activeforums_Topics.TopicId INNER JOIN
            {databaseOwner}[{objectQualifier}activeforums_ForumTopics] ON {databaseOwner}{objectQualifier}activeforums_Topics.TopicId = {databaseOwner}{objectQualifier}activeforums_ForumTopics.TopicId
		
		WHERE {databaseOwner}{objectQualifier}activeforums_Content.DateUpdated = 
		(
			SELECT     
				MAX({databaseOwner}{objectQualifier}activeforums_Content.DateUpdated) AS LastDate
			FROM         
				{databaseOwner}[{objectQualifier}activeforums_Content] INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_Replies] ON {databaseOwner}{objectQualifier}activeforums_Content.ContentId = {databaseOwner}{objectQualifier}activeforums_Replies.ContentId INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_Topics] ON {databaseOwner}{objectQualifier}activeforums_Replies.TopicId = {databaseOwner}{objectQualifier}activeforums_Topics.TopicId INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_ForumTopics] ON {databaseOwner}{objectQualifier}activeforums_Topics.TopicId = {databaseOwner}{objectQualifier}activeforums_ForumTopics.TopicId
			WHERE 
				{databaseOwner}{objectQualifier}activeforums_ForumTopics.ForumId = {databaseOwner}{objectQualifier}activeforums_Forums.ForumId 
		)
			
	 )
GO

/* Update LastPostAuthorId of Forums */
UPDATE 
	{databaseOwner}[{objectQualifier}activeforums_Forums]
SET 
	LastPostAuthorId = 
	isnull((
		SELECT 
			max({databaseOwner}{objectQualifier}activeforums_Content.AuthorId)
		FROM	
			{databaseOwner}[{objectQualifier}activeforums_Content] INNER JOIN
            {databaseOwner}[{objectQualifier}activeforums_Replies] ON {databaseOwner}{objectQualifier}activeforums_Content.ContentId = {databaseOwner}{objectQualifier}activeforums_Replies.ContentId INNER JOIN
            {databaseOwner}[{objectQualifier}activeforums_Topics] ON {databaseOwner}{objectQualifier}activeforums_Replies.TopicId = {databaseOwner}{objectQualifier}activeforums_Topics.TopicId INNER JOIN
            {databaseOwner}[{objectQualifier}activeforums_ForumTopics] ON {databaseOwner}{objectQualifier}activeforums_Topics.TopicId = {databaseOwner}{objectQualifier}activeforums_ForumTopics.TopicId
		
		WHERE {databaseOwner}{objectQualifier}activeforums_Content.DateUpdated = 
		(
			SELECT     
				MAX({databaseOwner}{objectQualifier}activeforums_Content.DateUpdated) AS LastDate
			FROM         
				{databaseOwner}[{objectQualifier}activeforums_Content] INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_Replies] ON {databaseOwner}{objectQualifier}activeforums_Content.ContentId = {databaseOwner}{objectQualifier}activeforums_Replies.ContentId INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_Topics] ON {databaseOwner}{objectQualifier}activeforums_Replies.TopicId = {databaseOwner}{objectQualifier}activeforums_Topics.TopicId INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_ForumTopics] ON {databaseOwner}{objectQualifier}activeforums_Topics.TopicId = {databaseOwner}{objectQualifier}activeforums_ForumTopics.TopicId
			WHERE 
				{databaseOwner}{objectQualifier}activeforums_ForumTopics.ForumId = {databaseOwner}{objectQualifier}activeforums_Forums.ForumId 
		)			
	 ),-1)
GO

/* Update LastPostAuthorName of Forums */
UPDATE 
	{databaseOwner}[{objectQualifier}activeforums_Forums]
SET 
	LastPostAuthorName = 
	(
		SELECT 
			Displayname
		FROM	
			{databaseOwner}[{objectQualifier}Users]			
		
		WHERE {databaseOwner}{objectQualifier}activeforums_Forums.LastPostAuthorId = Users.UserID 		
	 )
GO


/* Update LastPostDate of Forums */
UPDATE 
	{databaseOwner}[{objectQualifier}activeforums_Forums]
SET 
	LastPostDate = 
	isnull((
		SELECT 
			max({databaseOwner}{objectQualifier}activeforums_Content.DateUpdated)
		FROM	
			{databaseOwner}[{objectQualifier}activeforums_Content] INNER JOIN
            {databaseOwner}[{objectQualifier}activeforums_Replies] ON {databaseOwner}{objectQualifier}activeforums_Content.ContentId = {databaseOwner}{objectQualifier}activeforums_Replies.ContentId INNER JOIN
            {databaseOwner}[{objectQualifier}activeforums_Topics] ON {databaseOwner}{objectQualifier}activeforums_Replies.TopicId = {databaseOwner}{objectQualifier}activeforums_Topics.TopicId INNER JOIN
            {databaseOwner}[{objectQualifier}activeforums_ForumTopics] ON {databaseOwner}{objectQualifier}activeforums_Topics.TopicId = {databaseOwner}{objectQualifier}activeforums_ForumTopics.TopicId
		
		WHERE {databaseOwner}{objectQualifier}activeforums_Content.DateUpdated = 
		(
			SELECT     
				MAX({databaseOwner}{objectQualifier}activeforums_Content.DateUpdated) AS LastDate
			FROM         
				{databaseOwner}[{objectQualifier}activeforums_Content] INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_Replies] ON {databaseOwner}{objectQualifier}activeforums_Content.ContentId = {databaseOwner}{objectQualifier}activeforums_Replies.ContentId INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_Topics] ON {databaseOwner}{objectQualifier}activeforums_Replies.TopicId = {databaseOwner}{objectQualifier}activeforums_Topics.TopicId INNER JOIN
                {databaseOwner}[{objectQualifier}activeforums_ForumTopics] ON {databaseOwner}{objectQualifier}activeforums_Topics.TopicId = {databaseOwner}{objectQualifier}activeforums_ForumTopics.TopicId
			WHERE 
				{databaseOwner}{objectQualifier}activeforums_ForumTopics.ForumId = {databaseOwner}{objectQualifier}activeforums_Forums.ForumId 
		)			
	 ),-1)
GO

update 
	{databaseOwner}{objectQualifier}activeforums_Content
set 
	Body=cast(replace(cast(body as nvarchar(max)),'&lt;','<') as ntext)

update
	{databaseOwner}{objectQualifier}activeforums_Content
set
	Body=cast(replace(cast(body as nvarchar(max)),'&gt;','>') as ntext)

update
	{databaseOwner}{objectQualifier}activeforums_Content
set 
	Body=cast(replace(cast(body as nvarchar(max)),'&nbsp;',' ') as ntext),
	Summary=cast(replace(cast(Summary as nvarchar(max)),'&nbsp;',' ') as ntext)

update
	{databaseOwner}{objectQualifier}activeforums_Content
set 
	Body=cast(replace(cast(body as nvarchar(max)),'&amp;nbsp;',' ') as ntext),
	Summary=cast(replace(cast(Summary as nvarchar(max)),'&amp;nbsp;',' ') as ntext)

update
	{databaseOwner}{objectQualifier}activeforums_Content
set 
	Body=cast(replace(cast(body as nvarchar(max)),'&quot;','"') as ntext),
	Summary=cast(replace(cast(Summary as nvarchar(max)),'&quot;','"') as ntext)

update
	{databaseOwner}{objectQualifier}activeforums_Content
set 
	Body=cast(replace(cast(body as nvarchar(max)),'&#39;','''') as ntext),
	Summary=cast(replace(cast(Summary as nvarchar(max)),'&#39;','''') as ntext)
 
GO 
 


SET NOCOUNT OFF
