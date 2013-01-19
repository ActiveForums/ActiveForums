using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.ActiveForums
{
    public class ForumRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("ActiveForums", "", "{controller}/{action}", new {}, new string[] { "DotNetNuke.Modules.ActiveForums" });
        }
    }
}

