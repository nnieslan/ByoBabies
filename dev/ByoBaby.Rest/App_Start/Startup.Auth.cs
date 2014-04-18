using ByoBaby.Rest.Providers;
using ByoBaby.Model.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Facebook;
using Microsoft.Owin.Security.Twitter;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;
using System.Collections.Generic;
using System.Web.Configuration;
using System.Linq;

namespace ByoBaby.Rest
{
    public partial class Startup
    {
        public const string ExternalCookieAuthenticationType = DefaultAuthenticationTypes.ExternalCookie;

        public const string ExternalOAuthAuthenticationType = "ExternalToken";

        static Startup()
        {
            PublicClientId = "self";

            

            UserManagerFactory = () => new UserManager<IdentityUser>(new UserStore<IdentityUser>(new ByoBabyRepository()));
            CookieOptions = new CookieAuthenticationOptions(); 
            //{ 
            //    AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
            //    LoginPath = new PathString("/api/Account/ExternalLogin"),
            //    Provider = new CookieAuthenticationProvider
            //    {
            //        OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
            //            validateInterval: TimeSpan.FromMinutes(30),
            //            regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
            //    }

            //};

            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId, UserManagerFactory),
                AuthorizeEndpointPath = new PathString("/api/Account/ExternalLogin"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(14),
                AllowInsecureHttp = true
            };
        }

        public static CookieAuthenticationOptions CookieOptions { get; private set; }

        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }

        public static Func<UserManager<IdentityUser>> UserManagerFactory { get; set; }

        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(CookieOptions);
            app.UseExternalSignInCookie(ExternalCookieAuthenticationType);

            // Enable the application to use bearer tokens to authenticate users
            app.UseOAuthBearerTokens(OAuthOptions);

            //TODO - Customize the options for SCOPE etc here for each provider

            app.UseTwitterAuthentication(
                consumerKey: WebConfigurationManager.AppSettings["TwitterApiKey"],
                consumerSecret: WebConfigurationManager.AppSettings["TwitterApiSecret"]);

            app.UseFacebookAuthentication(
                appId: WebConfigurationManager.AppSettings["FacebookAppId"],
                appSecret: WebConfigurationManager.AppSettings["FacebookAppSecret"]);

            app.UseGoogleAuthentication(
                clientId: WebConfigurationManager.AppSettings["GoogleClientId"],
                clientSecret: WebConfigurationManager.AppSettings["GoogleClientSecret"]);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            
        }
    }
}
