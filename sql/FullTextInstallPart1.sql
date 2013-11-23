
-- Note, we jump through some hoops here for azure compatibility
-- This procedure has to be created and then executed before the main full text search proc
-- can be created.  Otherwise it throws an error on creation when run as a script.

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'{databaseOwner}[{objectQualifier}activeforums_Search_ManageFullText]') AND type in (N'P', N'PC'))
DROP PROCEDURE {databaseOwner}[{objectQualifier}activeforums_Search_ManageFullText]
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
					exec sp_fulltext_column N'{databaseOwner}{objectQualifier}activeforums_Content', N'Summary', N'add', 1033  
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