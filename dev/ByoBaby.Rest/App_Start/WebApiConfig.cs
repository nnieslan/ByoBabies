﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ByoBaby.Rest
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);

            config.Routes.MapHttpRoute(
               name: "LoginLogoutApi",
               routeTemplate: "api/{controller}/{action}",
               defaults: new { controller = "account", action = "get" },
               constraints: new { controller = "account" });


            config.Routes.MapHttpRoute(
               name: "NotificationsApi",
               routeTemplate: "api/{controller}/{id}",
               defaults: new { controller = "notifications", id = RouteParameter.Optional },
               constraints: new { controller = "notifications" });

            config.Routes.MapHttpRoute(
               name: "RequestsApi",
               routeTemplate: "api/{controller}/{id}",
               defaults: new { controller = "requests", id = RouteParameter.Optional },
               constraints: new { controller = "requests" });


            config.Routes.MapHttpRoute(
                name: "ConversationsApi",
                routeTemplate: "api/{userId}/conversation/{conversationId}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional, controller = "blurb" },
                constraints: new { controller = "blurb" });

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{userId}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

        }
    }
}
