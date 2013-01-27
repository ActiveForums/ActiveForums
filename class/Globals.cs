namespace DotNetNuke.Modules.ActiveForums
{
	public enum AttachStores: int
	{
		FILESYSTEM,
		DATABASE
	}
	public enum CurrentUserTypes: int
	{
		Anon,
		Auth,
		ForumMod,
		Admin,
		SuperUser
	}
	public enum EditorTypes: int
	{
		TEXTBOX,
		ACTIVEEDITOR,
		HTMLEDITORPROVIDER
	}
	public enum HTMLPermittedUsers: int
	{
		AllUsers,
		AuthenticatedUsers,
		TrustedUsers,
		Moderators,
		Administrators
	}
	public enum AvatarTypes: int
	{
		LocalFile,
		ExternalLink,
		MultipleLocalFile,
		MultipleExternalLink
	}
	public enum SubscriptionTypes: int
	{
		Disabled,
		Instant,
		DailyDigest,
		WeeklyDigest
	}
	public enum TopicTypes: int
	{
		Topic,
		Poll
	}
	public enum EmailFormats: int
	{
		HTML,
		PlainText
	}
	public enum ProfileTypes: int
	{
		Disabled,
		Basic,
		Advanced,
		Social
	}
	public enum PMTypes: int
	{
		Disabled = 0,
		Core = 1,
		Ventrian = 2
		//Social = 3
	}
	public enum SpecialTokenTypes: int
	{
		AddThis //http://www.addthis.com

	}
	public enum TrustTypes: int
	{
		NotTrusted,
		Trusted
	}
	public class Globals
	{
		public static string DefaultAnonRoles
		{
			get
			{
                return Common.Globals.glbRoleUnauthUser + ";" + Common.Globals.glbRoleAllUsers + ";";
			}
		}

		public const string ModulePath = "~/DesktopModules/ActiveForums/";

		public const string ControlRegisterTag = "<%@ Register TagPrefix=\"am\" Namespace=\"DotNetNuke.Modules.ActiveForums.Controls\" Assembly=\"DotNetNuke.Modules.ActiveForums\" %>";
		public const string ControlRegisterAFTag = "<%@ Register TagPrefix=\"af\" Namespace=\"DotNetNuke.Modules.ActiveForums.Controls\" Assembly=\"DotNetNuke.Modules.ActiveForums\" %>";
		public const string SocialRegisterTag = "<%@ Register TagPrefix=\"social\" Namespace=\"Active.Modules.Social.Controls\" Assembly=\"Active.Modules.Social\" %>";

		public const int GroupCount = 10000000;
		public const int ForumCount = 10000000;
		public const int SiteCount = -1;

	}

	public class SettingKeys
	{
		public const string GeneralSettingsKey = "GEN";
		public const string Mode = "MODE";
		public const string PageSize = "PAGESIZE";
		public const string AllowUserPhotos = "ALLOWUSERPHOTOS";
		public const string AllowUserBio = "ALLOWUSERBIO";
		public const string AllowSubscribe = "ALLOWSUBSCRIBE";
		public const string UserNameDisplay = "USERNAMEDISPLAY";
		public const string DisableUserProfiles = "DISABLEUSERPROFILES";
		public const string ProfileType = "PROFILETYPE";
		public const string ProfileTabId = "PROFILETABID";
		public const string AllowAvatars = "ALLOWAVATARS";
		public const string AllowAvatarLinks = "ALLOWAVATARLINKS";
		public const string AvatarHeight = "AVATARHEIGHT";
		public const string AvatarWidth = "AVATARWIDTH";
		public const string AvatarDefault = "AVATARDEFAULT";
		public const string AllowSignatures = "ALLOWSIGNATURES";
		public const string StatsEnabled = "STATSENABLED";
		public const string StatsTemplate = "STATSTEMPLATE";
		public const string StatsCache = "STATSCACHE";
		public const string DateFormatString = "DATEFORMATSTRING";
		public const string TimeFormatString = "TIMEFORMATSTRING";
		public const string TimeZoneOffset = "TIMEZONEOFFSET";
		public const string UsersOnlineEnabled = "USERSONLINEENABLED";
		public const string MemberListMode = "MEMBERLISTMODE";
		public const string ForumTemplateId = "FORUMTEMPLATEID";
		public const string DisableAccountTab = "DISABLEACCOUNTTAB";
		public const string Theme = "THEME";
		public const string MailQueue = "MAILQUEUE";
		public const string FullText = "FULLTEXT";
		public const string AllowSubTypes = "ALLOWSUBTYPES";
		public const string TemplateCache = "TEMPLATECACHE";
		public const string FloodInterval = "FLOODINTERVAL";
		public const string EditInterval = "EDITINTERVAL";
		public const string LoggingLevel = "LOGGINGLEVEL";
		public const string DeleteBehavior = "DELETEBEHAVIOR";
		public const string ProdKey = "AMFORUMS";

#if SKU_ENTERPRISE
		public const string ProdSku = "AF43ENT";
#elif SKU_STANDARD
		public const string ProdSku = "AF43STD";
#elif SKU_EXPRESS
		public const string ProdSku = "AF43EXP";
#elif SKU_LITE
		public const string ProdSku = "AF43LITE";
#endif
		public const string EnablePoints = "ENABLEPOINTS";
		public const string TopicPointValue = "TOPICPOINTVALUE";
		public const string ReplyPointValue = "REPLYPOINTVALUE";
		public const string AnswerPointValue = "ANSWERPOINTVALUE";
		public const string ModPointValue = "MODPOINTVALUE";
		public const string MarkAnswerPointValue = "MARKANSWERPOINTVALUE";
		public const string PMType = "PMTYPE";
		public const string PMTabId = "PMTABID";
		public const string InstallDate = "INSTALLDATE";
		public const string IsInstalled = "INSTALLED";
		public const string ProfileVisibility = "PROFILEVISIBILITY";
		public const string AddThisAccount = "ADDTHISACCOUNT";
		public const string UseShortUrls = "SHORTURLS";
		public const string RequireCaptcha = "REQCAPTCHA";
		public const string UseSkinBreadCrumb = "USESKINBC";
		public const string EnableAutoLink = "AUTOLINK";
		public const string ActiveSocialTopicKey = "ASTAK";
		public const string ActiveSocialRepliesKey = "ASRAK";
		public const string EnableURLRewriter = "EURLR";
		public const string PrefixURLBase = "URLBASE";
		public const string PrefixURLTags = "URLTAGS";
		public const string PrefixURLCategories = "URLCATS";
		public const string PrefixURLOther = "URLOTHER";





		public const string AdminResourceFile = "~/DesktopModules/ActiveForums/App_LocalResources/AdminResources.resx";
		public const string SharedResourceFile = "~/DesktopModules/ActiveForums/App_LocalResources/SharedResources.resx";
		public const string CacheDependencyFile = "~/DesktopModules/ActiveForums/cache/cachedep.resources";
		public const string TemplatePath = "~/DesktopModules/ActiveForums/config/templates/";







	}
	public class ForumSettingKeys
	{
		public const string AllowHTML = "ALLOWHTML";
		public const string AllowScript = "ALLOWSCRIPT";
		public const string AllowSubscribe = "ALLOWSUBSCRIBE";
		public const string AllowEmoticons = "ALLOWEMOTICONS";
		public const string AllowPostIcon = "ALLOWPOSTICON";
		public const string EditorType = "EDITORTYPE";
		public const string EditorWidth = "EDITORWIDTH";
		public const string EditorHeight = "EDITORHEIGHT";
		public const string EditorToolbar = "EDITORTOOLBAR";
		public const string EditorStyle = "EDITORSTYLE";
		public const string EditorPermittedUsers = "EDITORPERMITTEDUSERS";
		public const string AttachCount = "ATTACHCOUNT";
		public const string AttachMaxSize = "ATTACHMAXSIZE";
		public const string AttachTypeAllowed = "ATTACHTYPEALLOWED";
		public const string AttachStore = "ATTACHSTORE";
		public const string AttachMaxHeight = "ATTACHMAXHEIGHT";
		public const string AttachMaxWidth = "ATTACHMAXWIDTH";
		public const string AttachUniqueFileNames = "ATTACHUNIQUEFILENAMES";
		public const string IndexContent = "INDEXCONTENT";
		public const string AllowRSS = "ALLOWRSS";
		public const string TopicsTemplateId = "TOPICSTEMPLATEID";
		public const string TopicTemplateId = "TOPICTEMPLATEID";
		public const string IsModerated = "ISMODERATED";
		public const string AutoTrustLevel = "AUTOTRUSTLEVEL";
		public const string DefaultTrustValue = "DEFAULTTRUSTLEVEL";
		public const string ModApproveTemplateId = "MODAPPROVETEMPLATEID";
		public const string ModRejectTemplateId = "MODREJECTTEMPLATEID";
		public const string ModMoveTemplateId = "MODMOVETEMPLATEID";
		public const string ModDeleteTemplateId = "MODDELETETEMPLATEID";
		public const string ModNotifyTemplateId = "MODNOTIFYTEMPLATEID";
		public const string EmailAddress = "EMAILADDRESS";
		public const string UseFilter = "USEFILTER";
		public const string AllowAttach = "ALLOWATTACH";
		public const string TopicFormId = "TOPICFORMID";
		public const string ReplyFormId = "REPLYFORMID";
		public const string QuickReplyFormId = "QUICKREPLYFORMID";
		public const string ProfileTemplateId = "PROFILETEMPLATEID";
		public const string AutoSubscribeEnabled = "AUTOSUBSCRIBEENABLED";
		public const string AutoSubscribeRoles = "AUTOSUBSCRIBEROLES";
		public const string AutoSubscribeNewTopicsOnly = "AUTOSUBSCRIBENEWTOPICSONLY";
		public const string AllowTags = "ALLOWTAGS";

		public const string ActiveSocialEnabled = "AMASON";
		public const string ActiveSocialTopicsOnly = "AMASTO";
		public const string ActiveSocialSecurityOption = "AMASSEC";


		public const string MCEnabled = "MCENABLED";
		public const string MCUrl = "MCURL";
		public const string MCAddress = "MCADDRESS";
		public const string MCRestrictByAlias = "MCRESTRICTALIAS";
		public const string MCPop3UserName = "MCPOPUSERNAME";
		public const string MCPop3Password = "MCPOPPASSWORD";
		public const string MCPop3Server = "MCPOPSERVER";
		public const string MCAutoResponseTemplateId = "MCAUTORESPONSE";
		public const string MCAdminNotifyTemplateId = "MCADMINNOTIFY";
		public const string MCSubNotifyTemplateId = "MCSUBNOTIFY";
		public const string MCRejectTemplateId = "MCREJECTNOTIFY";
		public const string MCAutoCreateUsers = "MCAUTOCREATEUSERS";
		public const string MCModType = "MCMODTYPE";
		public const string MCEOMTag = "MCEOMTAG";
		public const string MCEOMTagRequired = "MCEOMTAGREQ";
		public const string MCRemoveHTML = "MCSTRIPHTML";

	}
	public class ParamKeys
	{
		public const string ForumId = "aff";
		public const string GroupId = "afg";
		public const string TopicId = "aft";
		public const string ReplyId = "afr";
		public const string ViewType = "afv";
		public const string QuoteId = "afq";
		public const string PageId = "afpg";
		public const string PostId = "postid";
		public const string Sort = "afs";
		public const string PageJumpId = "afpg";
		public const string ContentJumpId = "afc";
		public const string ConfirmActionId = "afca";
		public const string Tags = "aftg";
		public const string FirstNewPost = "afnp";
	}
	public class Views
	{
		public const string Topics = "topicsview";
		public const string Topic = "topic";
		public const string ForumView = "forumview";
		public const string TopicNew = "topicnew";
		public const string TopicEdit = "topicedit";
	}
	public class CacheKeys
	{
		public const string Rewards = "afrwd{0}";
		public const string PostInfo = "afpi{0}";
		public const string ForumInfo = "affi{0}";
		public const string ForumInfoWithUser = "affi{0}-{1}"; // KR
		public const string AllSettings = "afset{0}"; // KR
		public const string MainSettings = "afms{0}";
		public const string GroupInfo = "afgi{0}";
		public const string ProfileTemplate = "afpit{0}";
		public const string ForumList = "affl{0}";
	}
	public enum ConfirmActions: int
	{
		TopicSaved,
		TopicDeleted,
		ReplySaved,
		ReplyDeleted,
		MessagePending,
		MessageMoved,
		MessageDeleted,
		SendToComplete,
		SendToFailed,
		AlertSent

	}
	public class SortColumns
	{
		public const string ReplyCreated = "ReplyCreated";
		public const string TopicCreated = "TopicCreated";
	}
}
