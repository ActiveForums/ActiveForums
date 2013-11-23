IF NOT EXISTS (Select * From {databaseOwner}{objectQualifier}Schedule WHERE TypeFullName = 'DotNetNuke.Modules.ActiveForums.QueueScheduler,Active.Modules.Forums')
	INSERT INTO {databaseOwner}{objectQualifier}Schedule (TypeFullName,TimeLapse,TimeLapseMeasurement,RetryTimeLapse,RetryTimeLapseMeasurement,RetainHistoryNum,AttachToEvent,CatchUpEnabled,Enabled,ObjectDependencies,Servers)
	VALUES('DotNetNuke.Modules.ActiveForums.QueueScheduler,Active.Modules.Forums'
	,1,'m',3,'m',5,'',0,1,'','')
GO
IF NOT EXISTS (Select * From {databaseOwner}{objectQualifier}Schedule WHERE TypeFullName = 'DotNetNuke.Modules.ActiveForums.DailyDigest, ACTIVE.MODULES.FORUMS')
	INSERT INTO {databaseOwner}{objectQualifier}Schedule (TypeFullName,TimeLapse,TimeLapseMeasurement,RetryTimeLapse,RetryTimeLapseMeasurement,RetainHistoryNum,AttachToEvent,CatchUpEnabled,Enabled,ObjectDependencies,Servers)
		VALUES('DotNetNuke.Modules.ActiveForums.DailyDigest, ACTIVE.MODULES.FORUMS',1,'d',15,'m',5,'',0,0,'','')
GO
IF NOT EXISTS (Select * From {databaseOwner}{objectQualifier}Schedule WHERE TypeFullName = 'DotNetNuke.Modules.ActiveForums.WeeklyDigest, ACTIVE.MODULES.FORUMS')
	INSERT INTO {databaseOwner}{objectQualifier}Schedule (TypeFullName,TimeLapse,TimeLapseMeasurement,RetryTimeLapse,RetryTimeLapseMeasurement,RetainHistoryNum,AttachToEvent,CatchUpEnabled,Enabled,ObjectDependencies,Servers)
		VALUES('DotNetNuke.Modules.ActiveForums.WeeklyDigest, ACTIVE.MODULES.FORUMS'
	,7,'d',15,'m',5,'',0,0,'','')
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'{databaseOwner}{objectQualifier}AF_TopicsForDigest'))
DROP VIEW {databaseOwner}{objectQualifier}AF_TopicsForDigest
GO
CREATE VIEW {databaseOwner}{objectQualifier}AF_TopicsForDigest
AS
SELECT     P.PostID, P.ForumID, P.Subject, P.Body, U.Username, U.FirstName, U.LastName, U.DisplayName, P.DateAdded, F.Name AS ForumName, 
                      FG.GroupName, T.TabID, N.AllowSubTypes, N.PortalID, N.ModuleID, P.Views, P.Replies, N.UserNameDisplay, U.Email, Admin.Email AS FromEmail, 
                      F.ForumGroupID
FROM         {databaseOwner}{objectQualifier}TabModules AS T INNER JOIN
                      {databaseOwner}{objectQualifier}NTForums AS N ON T.ModuleID = N.ModuleID INNER JOIN
                      {databaseOwner}{objectQualifier}NTForums_Posts AS P INNER JOIN
                      {databaseOwner}{objectQualifier}Users AS U ON P.UserID = U.UserID INNER JOIN
                      {databaseOwner}{objectQualifier}NTForums_Forums AS F ON P.ForumID = F.ForumID INNER JOIN
                      {databaseOwner}{objectQualifier}NTForums_ForumGroups AS FG ON F.ForumGroupID = FG.ForumGroupID ON N.ModuleID = FG.ModuleID INNER JOIN
                      {databaseOwner}{objectQualifier}Portals AS Portal ON N.PortalID = Portal.PortalID INNER JOIN
                      {databaseOwner}{objectQualifier}Users AS Admin ON Portal.AdministratorId = Admin.UserID
WHERE     (P.ParentPostID = 0) AND (P.Deleted = 0) AND (P.Approved = 1)

GO
IF  EXISTS (SELECT * FROM dbo.sysobjects where id = object_id(N'{databaseOwner}{objectQualifier}ActiveForums_GetDigest') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE {databaseOwner}{objectQualifier}ActiveForums_GetDigest
GO
CREATE PROCEDURE {databaseOwner}{objectQualifier}ActiveForums_GetDigest(@SubscriptionType varchar(10), @DateStart datetime)
AS
BEGIN
DECLARE @SubType varchar(10)
SET @SubType = '%' + @SubscriptionType + '%'
SELECT     TD.PostID, TD.ForumID, TD.Subject, TD.Body, TD.Username, TD.FirstName, TD.LastName, TD.DisplayName, TD.DateAdded, TD.ForumName, 
                      TD.GroupName, TD.TabID, TD.PortalID, U.Username AS SubscriberUserName, U.FirstName AS SubscriberFirstName, 
                      U.LastName AS SubscriberLastname, U.DisplayName AS SubscriberDisplayName, U.Email, TD.ModuleId, TD.Replies, TD.Views,TD.UserNameDisplay,
						TD.FromEmail,TD.ForumGroupId

FROM         {databaseOwner}{objectQualifier}AF_TopicsForDigest AS TD INNER JOIN
                      {databaseOwner}{objectQualifier}NTForums_ForumSubscriptions AS S ON TD.ForumID = S.ForumID INNER JOIN
                      {databaseOwner}{objectQualifier}NTForums_UserDetails AS UD INNER JOIN
                      {databaseOwner}{objectQualifier}Users AS U ON UD.UserID = U.UserID ON S.UserID = U.UserID
WHERE     (Convert(varchar(20),UD.SubscriptionType) = @SubscriptionType) AND (TD.AllowSubTypes LIKE @SubType) AND (Convert(varchar(20),TD.DateAdded,102) >= Convert(varchar(20),@DateStart,102))
Order By TD.ModuleId, U.Email, TD.GroupName, TD.ForumName, TD.DateAdded
END
GO
