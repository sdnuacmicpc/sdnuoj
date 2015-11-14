using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using SDNUOJ.Controllers;

namespace SDNUOJ
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            ScheduleTaskRegistration.RegisterAllScheduleTasks();
            AreaRegistration.RegisterAllAreas();
            GlobalFiltersRegistration.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalRoutesTable.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            GlobalAuthenticateModule.ReplaceFormAuthenticateModel(this.Context);
        }

        public override String GetVaryByCustomString(HttpContext context, String custom)
        {
            custom = custom.ToLowerInvariant();

            if (String.Equals(custom, "nm"))
            {
                return "nm:" + context.User.Identity.Name + ";";
            }
            else if (String.Equals(custom, "in"))
            {
                return (context.User.Identity.IsAuthenticated ? "in=t;" : "in=f;");
            }
            else if (String.Equals(custom, "pm"))
            {
                return (context.User.IsInRole("ProblemManage") ? "pm=t;" : "pm=f;");
            }
            else if (String.Equals(custom, "sa"))
            {
                return (context.User.IsInRole("SuperAdministrator") ? "sa=t;" : "sa=f;");
            }
            else
            {
                return base.GetVaryByCustomString(context, custom);
            }
        }
    }
}
