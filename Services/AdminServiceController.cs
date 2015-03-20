//
// Active Forums - http://www.dnnsoftware.com
// Copyright (c) 2013
// by DNN Corp.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
// to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions 
// of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED 
// TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
// THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
// CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
//
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;


namespace DotNetNuke.Modules.ActiveForums
{
    [ValidateAntiForgeryToken]
    [SupportedModules("Active Forums")]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
    public class AdminServiceController : DnnApiController
    {
        // DTO for ToggleUrlHandler
        public class ToggleUrlHandlerDTO
        {
            public int ModuleId { get; set; }
        }

        public HttpResponseMessage ToggleURLHandler(ToggleUrlHandlerDTO dto)
        {
            var objModules = new Entities.Modules.ModuleController();
            var objSettings = new SettingsInfo { MainSettings = objModules.GetModuleSettings(dto.ModuleId) };
            var cfg = new ConfigUtils();
            bool success;
            if (Utilities.IsRewriteLoaded())
            {
                cfg.DisableRewriter(HttpContext.Current.Server.MapPath("~/web.config"));
                return Request.CreateResponse(HttpStatusCode.OK, "disabled");
            }

            cfg.EnableRewriter(HttpContext.Current.Server.MapPath("~/web.config"));
            return Request.CreateResponse(HttpStatusCode.OK, "enabled");
        }

        //DTO for RunMaintenance
        public class RunMaintenanceDTO
        {
            public int ModuleId { get; set; }
            public int ForumId { get; set; }
            public int OlderThan { get; set; }
            public int ByUserId { get; set; }
            public int LastActive { get; set; }
            public bool WithNoReplies { get; set; }
            public bool DryRun { get; set; }
        }

        public HttpResponseMessage RunMaintenance(RunMaintenanceDTO dto)
        {
            var objModules = new Entities.Modules.ModuleController();
            var objSettings = new SettingsInfo {MainSettings = objModules.GetModuleSettings(dto.ModuleId)};
            var rows = DataProvider.Instance().Forum_Maintenance(dto.ForumId, dto.OlderThan, dto.LastActive, dto.ByUserId, dto.WithNoReplies, dto.DryRun, objSettings.DeleteBehavior);
            if (dto.DryRun)
                return Request.CreateResponse(HttpStatusCode.OK, new { Result = string.Format(Utilities.GetSharedResource("[RESX:Maint:DryRunResults]", true), rows.ToString()) });

            return Request.CreateResponse(HttpStatusCode.OK, new { Result = Utilities.GetSharedResource("[RESX:ProcessComplete]", true) });
        }

        public string GetSecurityGrid(int groupId, int forumId)  // Needs DTO
        {
            var sb = new StringBuilder();

            return sb.ToString();
        }

        public class UserProfileDTO
        {
            public int UserId { get; set; }
            public int? TrustLevel { get; set; }
            public string UserCaption { get; set; }
            public string Signature { get; set; }
            public int? RewardPoints { get; set; }
        }

        [HttpGet]
        public HttpResponseMessage GetUserProfile(int userId)
        {
            var upc = new UserProfileController();
            var up = upc.Profiles_Get(PortalSettings.PortalId, ActiveModule.ModuleID, userId);

            if(up == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            var result = new
                             {
                                 up.UserID,
                                 up.TrustLevel,
                                 up.UserCaption,
                                 up.Signature,
                                 up.RewardPoints
                             };

            return Request.CreateResponse(HttpStatusCode.OK, result);
        }

        [HttpPost]
        public HttpResponseMessage UpdateUserProfile(UserProfileDTO dto)
        {
            var upc = new UserProfileController();
            var up = upc.Profiles_Get(PortalSettings.PortalId, ActiveModule.ModuleID, dto.UserId);

            if (up == null)
                return Request.CreateResponse(HttpStatusCode.NotFound);

            if (dto.TrustLevel.HasValue)
                up.TrustLevel = dto.TrustLevel.Value;

            up.UserCaption = dto.UserCaption;
            up.Signature = dto.Signature;

            if (dto.RewardPoints.HasValue)
                up.RewardPoints = dto.RewardPoints.Value;

            upc.Profiles_Save(up);

            return Request.CreateResponse(HttpStatusCode.OK);
        }

        //DTO for ToggleSecurity
        public class ToggleSecurityDTO
        {
            public int ModuleId { get; set; }
            public string Action { get; set; }
            public int PermissionsId { get; set; }
            public string SecurityId { get; set; }
            public int SecurityType { get; set; }
            public string SecurityKey { get; set; }
            public string ReturnId { get; set; }
        }

        [HttpPost]
        public HttpResponseMessage ToggleSecurity(ToggleSecurityDTO dto)
        {
            var db = new Data.Common();
            var sb = new StringBuilder();
            switch (dto.Action)
            {
                case "delete":
                    {
                        Permissions.RemoveObjectFromAll(dto.SecurityId, dto.SecurityType, dto.PermissionsId);
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                case "addobject":
                    {
                        if (dto.SecurityType == 1)
                        {
                            var uc = new UserController();
                            var ui = uc.GetUser(PortalSettings.PortalId, dto.ModuleId, dto.SecurityId);
                            dto.SecurityId = ui != null ? ui.UserId.ToString() : string.Empty;
                        }
                        else
                        {
                            if (dto.SecurityId.Contains(":"))
                                dto.SecurityType = 2;
                        }
                        if (!(string.IsNullOrEmpty(dto.SecurityId)))
                        {
                            var permSet = db.GetPermSet(dto.PermissionsId, "View");
                            permSet = Permissions.AddPermToSet(dto.SecurityId, dto.SecurityType, permSet);
                            db.SavePermSet(dto.PermissionsId, "View", permSet);
                        }

                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                default:
                    {
                        var permSet = db.GetPermSet(dto.PermissionsId, dto.SecurityKey);
                        if (dto.Action == "remove")
                            permSet = Permissions.RemovePermFromSet(dto.SecurityId, dto.SecurityType, permSet);
                        else
                            permSet = Permissions.AddPermToSet(dto.SecurityId, dto.SecurityType, permSet);

                        db.SavePermSet(dto.PermissionsId, dto.SecurityKey, permSet);
                        return Request.CreateResponse(HttpStatusCode.OK, dto.Action + "|" + dto.ReturnId);
                    }
            }
        }

    }
}
