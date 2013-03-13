using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ByoBaby.Rest.Controllers;
using ByoBaby.Model;


namespace ByoBaby.Rest.Test.Controllers
{
    internal class ControllerTestHelper
    {
        internal static T InitializeController<T>(
            HttpMethod verb,
            string url,
            string controllerName,
            string routeName,
            string routeTemplate,
            HttpRouteValueDictionary routeValues) where T : ApiController, new()
        {
            var config = new HttpConfiguration();
            var request = new HttpRequestMessage(verb, url);
            var route = config.Routes.MapHttpRoute(
                name: routeName,
                routeTemplate: routeTemplate,
                defaults: new { id = RouteParameter.Optional });

            var data = new HttpRouteData(
                route, routeValues);

            var controller = new T()
            {
                ControllerContext = new System.Web.Http.Controllers.HttpControllerContext(config, data, request),
                Request = request
            };

            controller.Request.Properties.Add(HttpPropertyKeys.HttpRouteDataKey, data);
            controller.Request.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;

            return controller;
        }


    }
}
