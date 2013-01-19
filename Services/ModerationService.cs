using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Services.Journal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DotNetNuke.Web.Api;
using System.Web;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Social.Notifications;
using DotNetNuke.Services.Journal;
using DotNetNuke.Services.FileSystem;
using System.Globalization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using DotNetNuke.Common;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Content.Common;
using DotNetNuke.Entities.Host;
using DotNetNuke.Entities.Users;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using DotNetNuke.Web.Api;
using System.Linq;


namespace DotNetNuke.Modules.ActiveForums
{
    public class ModerationServiceController : DnnApiController
    {
        private int TabId = -1;
        private int ModuleId = -1;
        private int ForumId = -1;
        private int TopicId = -1;
        private int ReplyId = -1;

        [DnnAuthorize()]
        public HttpResponseMessage ApprovePost(int notificationId)
        {
            Notification notify = NotificationsController.Instance.GetNotification(notificationId);
            ParseKey(notify.Context);
            ForumController fc = new ForumController();
            Forum fi = fc.Forums_Get(ForumId, -1, false, true);
            if (fi == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Forum Not Found");
                //return Json(new {Result = "error"});
            }

            if (!(IsMod(ForumId)))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "User is not a moderator for this forum");
                //return Json(new {Result = "error"});
            }
            if (ReplyId > 0)
            {
                ReplyController rc = new ReplyController();
                ReplyInfo reply = rc.ApproveReply(PortalSettings.PortalId, TabId, ModuleId, ForumId, TopicId, ReplyId);
                if (reply == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Reply Not Found");
                    //return Json(new {Result = "error"});
                }
            }
            else
            {
                TopicsController tc = new TopicsController();
                TopicInfo topic = tc.ApproveTopic(PortalSettings.PortalId, TabId, ModuleId, ForumId, TopicId);
                if (topic == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Topic Not Found");
                    //return Json(new {Result = "error"});
                }

            }
            NotificationsController.Instance.DeleteNotification(notificationId);
            return Request.CreateResponse(HttpStatusCode.OK, "success");
            //return Json(new {Result = "success"});
        }

        [DnnAuthorize()]
        public HttpResponseMessage RejectPost(int notificationId)
        {
            Notification notify = NotificationsController.Instance.GetNotification(notificationId);
            ParseKey(notify.Context);
            ForumController fc = new ForumController();
            Forum fi = fc.Forums_Get(ForumId, -1, false, true);
            if (fi == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Forum Not Found");
                //return Json(new { Result = "error" });
            }
            if (!(IsMod(ForumId)))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "User is not a moderator for this forum");
                //return Json(new { Result = "error" });
            }

            ModController mc = new ModController();
            mc.Mod_Reject(PortalSettings.PortalId, ModuleId, UserInfo.UserID, ForumId, TopicId, ReplyId);
            int authorId = -1;
            if (ReplyId > 0)
            {
                ReplyController rc = new ReplyController();
                ReplyInfo reply = rc.Reply_Get(PortalSettings.PortalId, ModuleId, TopicId, ReplyId);

                if (reply == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Reply Not Found");
                    //return Json(new { Result = "error" });
                }
                authorId = reply.Content.AuthorId;
            }
            else
            {
                TopicsController tc = new TopicsController();
                TopicInfo topic = tc.Topics_Get(PortalSettings.PortalId, ModuleId, TopicId);
                if (topic == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Topic Not Found");
                }
                authorId = topic.Content.AuthorId;
            }
            if (fi.ModRejectTemplateId > 0 & authorId > 0)
            {
                DotNetNuke.Entities.Users.UserController uc = new DotNetNuke.Entities.Users.UserController();
                DotNetNuke.Entities.Users.UserInfo ui = uc.GetUser(PortalSettings.PortalId, authorId);
                if (ui != null)
                {
                    Author au = new Author();
                    au.AuthorId = authorId;
                    au.DisplayName = ui.DisplayName;
                    au.Email = ui.Email;
                    au.FirstName = ui.FirstName;
                    au.LastName = ui.LastName;
                    au.Username = ui.Username;
                    Email oEmail = new Email();
                    oEmail.SendEmail(fi.ModRejectTemplateId, PortalSettings.PortalId, ModuleId, TabId, ForumId, TopicId, ReplyId, string.Empty, au);
                }

            }
            NotificationsController.Instance.DeleteNotification(notificationId);
            return Request.CreateResponse(HttpStatusCode.OK, "success");
            //return Json(new { Result = "success" });
        }

        [DnnAuthorize()]
        public HttpResponseMessage DeletePost(int notificationId)
        {
            Notification notify = NotificationsController.Instance.GetNotification(notificationId);
            ParseKey(notify.Context);
            ForumController fc = new ForumController();
            Forum fi = fc.Forums_Get(ForumId, -1, false, true);
            if (fi == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Forum Not Found");
                //return Json(new { Result = "error locating forum" });
            }
            if (!(IsMod(ForumId)))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "User is not a moderator for this forum");
                //return Json(new { Result = "error user not authorized" });
            }
            int authorId = -1;
            if (fi.ModDeleteTemplateId > 0)
            {
                try
                {
                    Email oEmail = new Email();
                    oEmail.SendEmail(fi.ModDeleteTemplateId, PortalSettings.PortalId, ModuleId, TabId, ForumId, TopicId, ReplyId, string.Empty, null);
                }
                catch (Exception ex)
                {
                }
            }
            SettingsInfo ms = DataCache.MainSettings(ModuleId);
            if (ReplyId > 0 & ReplyId != TopicId)
            {
                ReplyController rc = new ReplyController();
                ReplyInfo reply = rc.Reply_Get(PortalSettings.PortalId, ModuleId, TopicId, ReplyId);

                if (reply == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Reply Not Found");
                    //return Json(new { Result = "error" });
                }
                authorId = reply.Content.AuthorId;
                rc.Reply_Delete(PortalSettings.PortalId, ForumId, TopicId, ReplyId, ms.DeleteBehavior);
            }
            else
            {
                TopicsController tc = new TopicsController();
                TopicInfo topic = tc.Topics_Get(PortalSettings.PortalId, ModuleId, TopicId);
                if (topic == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Topic Not Found");
                    //return Json(new { Result = "error" });
                }
                authorId = topic.Content.AuthorId;
                tc.Topics_Delete(PortalSettings.PortalId, ModuleId, ForumId, TopicId, ms.DeleteBehavior);

            }
            NotificationsController.Instance.DeleteNotification(notificationId);
            return Request.CreateResponse(HttpStatusCode.OK, "success");
            //return Json(new { Result = "success" });

        }

        [DnnAuthorize()]
        public HttpResponseMessage IgnorePost(int notificationId)
        {
            Notification notify = NotificationsController.Instance.GetNotification(notificationId);
            ParseKey(notify.Context);
            ForumController fc = new ForumController();
            Forum fi = fc.Forums_Get(ForumId, -1, false, true);
            if (fi == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, "Forum Not Found");
                //return Json(new { Result = "error" });
            }
            if (!(IsMod(ForumId)))
            {
                return Request.CreateResponse(HttpStatusCode.Forbidden, "User is not a moderator for this forum");
                //return Json(new { Result = "error" });
            }

            NotificationsController.Instance.DeleteNotification(notificationId);
            return Request.CreateResponse(HttpStatusCode.OK, "success");
            //return Json(new { Result = "success" });
        }

        #region Private Methods
        private bool IsMod(int ForumId)
        {
            List<UserInfo> mods = Utilities.GetListOfModerators(PortalSettings.PortalId, ForumId);

            int count = mods.Where(i => i.UserID == UserInfo.UserID).Count();

            return (count > 0);

            //foreach (UserInfo u in mods)
            //{
            //    if (UserInfo.UserID == u.UserID)
            //    {
            //        return true;
            //        break;
            //    }
            //}
            //return false;
        }
        private void ParseKey(string key)
        {
            string[] keys = key.Split(':');
            TabId = int.Parse(keys[0]);
            ModuleId = int.Parse(keys[1]);
            ForumId = int.Parse(keys[2]);
            TopicId = int.Parse(keys[3]);
            ReplyId = int.Parse(keys[4]);
        }
        #endregion
    }
}

