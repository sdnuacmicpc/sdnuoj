using System;
using System.Web.Mvc;
using System.Web.Routing;

using LowercaseRoutesMVC;

namespace SDNUOJ.Controllers
{
    public class GlobalRoutesTable
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRouteLowercase(
                name: "TopicPage",
                url: "page/{name}",
                defaults: new { controller = "TopicPage", action = "Index" },
                namespaces: new String[] { "SDNUOJ.Controllers" }
            );

            routes.MapRouteLowercase(
                name: "ProblemStatistic",
                url: "problem/statistic/{pid}/{id}/{lang}/{order}",
                defaults: new { controller = "Problem", action = "Statistic", id = UrlParameter.Optional, lang = UrlParameter.Optional, order = UrlParameter.Optional },
                constraints: new { pid = @"\d+" },
                namespaces: new String[] { "SDNUOJ.Controllers" }
            );

            routes.MapRouteLowercase(
                name: "ProblemForum",
                url: "problem/forum/{pid}/{id}",
                defaults: new { controller = "Forum", action = "Problem", id = UrlParameter.Optional },
                constraints: new { pid = @"\d+" },
                namespaces: new String[] { "SDNUOJ.Controllers" }
            );

            routes.MapRouteLowercase(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new String[] { "SDNUOJ.Controllers" }
            );
        }
    }
}