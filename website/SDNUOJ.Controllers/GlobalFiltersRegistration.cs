using System;
using System.Web;
using System.Web.Mvc;

namespace SDNUOJ.Controllers
{
    public class GlobalFiltersRegistration
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}