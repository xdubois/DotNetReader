using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DotNetReader
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "View Details",
                url: "Feed/Views/{slug}",
                defaults: new { controller = "Feed", action = "Views", slug = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "Update deamon",
                url: "Feed/UpdateAll/{synchroCode}",
                defaults: new { controller = "Feed", action = "UpdateAll", synchroCode = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

        
        }
    }
}