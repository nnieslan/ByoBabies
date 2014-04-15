using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Castle.Facilities.TypedFactory;
using System.Reflection;

namespace ByoBaby.Rest.Windsor
{
    //public class ViewModelWindsorInstaller : IWindsorInstaller
    //{
    //    public void Install(IWindsorContainer container, IConfigurationStore store)
    //    {
    //        container.Register(Component.For<IWindsorViewModelFactory>().AsFactory(c => c.SelectedWith(new DefaultTypedFactoryComponentSelector())),
    //            Classes.FromThisAssembly()
    //            .BasedOn<ISurveyViewModel>()
    //            .WithServiceBase()
    //            .Configure(c => c.Named(c.Implementation.Name))
    //            .LifestyleTransient());

    //    }

    //    public class ViewModelTypedFactoryComponentSelector : DefaultTypedFactoryComponentSelector
    //    {
    //        protected override string GetComponentName(MethodInfo method, object[] arguments)
    //        {
    //            if (method.Name == "Create")
    //            {
    //                var modelName = arguments[0].ToString() + "SurveyViewModel";
    //                return modelName;
    //            }
    //            return base.GetComponentName(method, arguments);
    //        }
    //    }
    //}
}