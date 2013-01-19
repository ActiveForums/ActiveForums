using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.ActiveForums
{
    public class ForumRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("ActiveForums", "default", "{controller}/{action}", new { Controller = "ModerationService", Action = "Index" }, new string[] { "DotNetNuke.Modules.ActiveForums" });
        }
    }
}

