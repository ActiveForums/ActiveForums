using DotNetNuke.Web.Api;

namespace DotNetNuke.Modules.ActiveForums
{
    public class ForumRouteMapper : IServiceRouteMapper
    {
        public void RegisterRoutes(IMapRoute mapRouteManager)
        {
            mapRouteManager.MapHttpRoute("ActiveForums", "default", "{controller}/{action}", new {}, new[] { "DotNetNuke.Modules.ActiveForums" });
        }
    }
}

