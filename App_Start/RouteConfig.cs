using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DigitalTVBilling
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Abonent_Edit",
                url: "{controller}/{action}/{id}/{cur_card}",
                defaults: new { controller = "Abonent", action = "Edit", id = UrlParameter.Optional, cur_card = UrlParameter.Optional }
            );

        }
    }
}
