using System;
using System.Web.Mvc;

using LowercaseRoutesMVC;

namespace SDNUOJ.Areas.Contest
{
    public class ContestAreaRegistration : AreaRegistration 
    {
        public override String AreaName 
        {
            get 
            {
                return "Contest";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRouteLowercase(
                name: "Contest_ProblemStatistic",
                url: "contest/{cid}/problem/statistic/{pid}/{id}/{lang}/{order}",
                defaults: new { controller = "Problem", action = "Statistic", id = UrlParameter.Optional, lang = UrlParameter.Optional, order = UrlParameter.Optional },
                constraints: new { cid = @"\d+", pid = @"\d+" },
                namespaces: new String[] { "SDNUOJ.Areas.Contest.Controllers" }
            );

            context.MapRouteLowercase(
                name: "Contest_Default",
                url: "contest/{cid}/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { cid = @"\d+" },
                namespaces: new String[] { "SDNUOJ.Areas.Contest.Controllers" }
            );
        }
    }
}