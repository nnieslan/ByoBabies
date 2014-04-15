using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using ByoBaby.Model.Repositories;
using ByoBaby.Rest.Windsor;
using ByoBaby.Security;
using Castle.Windsor;

namespace ByoBaby.Rest
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        #region properties

        /// <summary>
        /// Gets a globally available internal instance of the IoC container for dependency resolution.
        /// </summary>
        internal static IWindsorContainer Container { get; private set; }


        #endregion

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            Database.SetInitializer<ByoBabyRepository>(new ByoBabyDataContextInitializer());
            WebApiApplication.Container = WindsorConfig.CreateContainer();

            AuthConfig.RegisterOpenAuth();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
           
            if (exception is HttpException)
            {
                //Logger.Log404(HttpContext.Current.Request.Url.ToString());
                //Logger.Error(string.Format("Unable to navigate to {0}", HttpContext.Current.Request.Url.ToString()), exception);
            }
            else
            {
                //Logger.Fatal("Uncaught exception", exception);
            }
            HttpContext.Current.Response.TrySkipIisCustomErrors = true;
        }

        /// <summary>
        /// Handles the PostAuthenticateRequest event for the application.
        /// </summary>
        /// <param name="sender">
        /// The sender for the event.
        /// </param>
        /// <param name="args">
        /// The <see cref="EventArgs"/> for the event.
        /// </param>
        protected void Application_PostAuthenticateRequest(object sender, EventArgs args)
        {                                       
            var user = HttpContext.Current.User;
            if (user.Identity.IsAuthenticated && user.Identity.AuthenticationType == "Forms")
            {
                var formsIdentity = (FormsIdentity)user.Identity;
                var userIdentity = new WebUserIdentity(formsIdentity.Ticket);
                var principal = new ByoBabiesUserPrincipal(userIdentity);

                ByoBabiesUserPrincipal.Current = principal;
            }
        }
    }
}