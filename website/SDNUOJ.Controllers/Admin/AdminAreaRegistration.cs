using System;
using System.Web.Mvc;

using LowercaseRoutesMVC;

namespace SDNUOJ.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override String AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRouteLowercase(
                name: "Admin_ContestUser",
                url: "admin/contest/userlist/{cid}/{id}",
                defaults: new { controller = "Contest", action = "UserList", id = UrlParameter.Optional },
                constraints: new { cid = @"\d+", id = @"\d+" },
                namespaces: new String[] { "SDNUOJ.Areas.Admin.Controllers" }
            );

            context.MapRouteLowercase(
                name: "Admin_Default",
                url: "admin/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new String[] { "SDNUOJ.Areas.Admin.Controllers" }
            );
        }
    }
}