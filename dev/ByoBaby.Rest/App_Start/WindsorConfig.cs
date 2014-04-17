using ByoBaby.Rest.Windsor;
using Castle.Windsor;
using System.Web.Http;
using System.Web.Http.Dispatcher;

namespace ByoBaby.Rest
{
    /// <summary>
    /// Static configuration class for bootstrapping the Windsor IOC container.  
    /// Should be called from App_Start once.
    /// </summary>
    public static class WindsorConfig
    {
        /// <summary>
        /// Creates the Windsor container for EgoBoomSite and does any registrations with the container for components
        /// used on the site.
        /// </summary>
        public static IWindsorContainer CreateContainer()
        {
            var container = new WindsorContainer();
            //// Add installers here
            //container.Install(
            //    new ControllersWindsorInstaller(), 
            //    new ServicesWindsorInstaller());

            //GlobalConfiguration.Configuration.Services.Replace(
            //    typeof(IHttpControllerActivator),
            //    new WindsorCompositionRoot(container));

            return container;
        }
    }
}