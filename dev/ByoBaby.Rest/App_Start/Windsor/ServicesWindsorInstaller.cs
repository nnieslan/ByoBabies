using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace ByoBaby.Rest.Windsor
{
    public class ServicesWindsorInstaller : IWindsorInstaller 
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            //// Services
            //container.Register(
            //    Component.For<IFollowerDomainService>().ImplementedBy<FollowerDomainService>().LifestyleTransient(),
            //    Component.For<IUrlShortenerService>().ImplementedBy<BitlyApiService>().LifestyleTransient(),
            //    Component.For<IInvitationDomainService>().ImplementedBy<InvitationDomainService>().LifestyleTransient(),
            //    Component.For<IUserThirdPartyContactsDomainService>().ImplementedBy<UserThirdPartyContactsDomainService>().LifestyleTransient(),
            //    Component.For<IUserDomainService>().ImplementedBy<UserDomainService>().LifestyleTransient(),
            //    Component.For<ISurveyDomainService>().ImplementedBy<SurveyDomainService>().LifestyleTransient()
            //);

            //// Repositories
            //container.Register(
            //    Component.For<IUserRepository>().ImplementedBy<UserRepository>().LifestyleTransient(),
            //    Component.For<IUserFollowRepository>().ImplementedBy<UserFollowRepository>().LifestyleTransient(),
            //    Component.For<ISurveyRepository>().ImplementedBy<SurveyRepository>().LifestyleTransient(),
            //    Component.For<IUserSurveyInviteRepository>().ImplementedBy<UserSurveyInviteRepository>().LifestyleTransient(),
            //    Component.For<IThirdPartyUserContactImportRepository>().ImplementedBy<ThirdPartyUserContactImportRepository>().LifestyleTransient()
            //);
        }
    }
}