
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;


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
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //   name: "LoginLogoutApi",
            //   routeTemplate: "api/{controller}/{action}",
            //   defaults: new { controller = "account", action = "get" },
            //   constraints: new { controller = "account" });


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
               name: "LocationsApi",
               routeTemplate: "api/{controller}/{action}",
               defaults: new { controller = "nearby", action = "getlocations" },
               constraints: new { controller = "nearby" });


            config.Routes.MapHttpRoute(
                name: "ConversationsApi",
                routeTemplate: "api/{userId}/conversation/{conversationId}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional, controller = "blurb" },
                constraints: new { controller = "blurb" });

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{userId}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            GlobalConfiguration
                .Configuration
                .Formatters
                .Insert(0, new ByoBaby.Rest.Helpers.JsonpFormatter());

        }
    }
}
