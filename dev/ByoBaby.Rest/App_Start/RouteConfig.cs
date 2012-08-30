using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Http;
using System.Web.Routing;


namespace ByoBaby.Rest
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //routes.MapRoute(
            //    name: "Default",
            //    url: "{controller}/{action}/{id}",
            //    defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            //);

            routes.MapHttpRoute(
               name: "LoginLogoutApi",
               routeTemplate: "api/{controller}/{action}",
               defaults: new { controller = "account", action = "get" },
               constraints: new { controller = "account" });

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{user}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional, user = RouteParameter.Optional});

            routes.MapHttpRoute(
                name: "ConversationsApi",
                routeTemplate: "api/{user}/conversation/{conversationId}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional, user = RouteParameter.Optional, controller = "blurb" },
                constraints: new { controller = "blurb" });

        }
    }
}