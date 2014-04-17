using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ByoBaby.Rest
{
    public static class AuthConfig
    {
        public static void RegisterOpenAuth()
        {
            // See http://go.microsoft.com/fwlink/?LinkId=252803 for details on setting up this ASP.NET
            // application to support logging in via external services.

            //OpenAuth.AuthenticationClients.Add("Twitter",
            //    () => new EbTwitterClient(new EbConsumerTokenManager(
            //    TwitterApiKey, 
            //    TwitterApiSecret, 
            //    ThirdPartyIdentityProviders.twitter)));

            //OpenAuth.AuthenticationClients.Add("Facebook", 
            //    () => new DotNetOpenAuth.AspNet.Clients.FacebookClient(
            //    "",//TODO GET API KEYS
            //    "",
            //    new string[] { "user_about_me", "user_birthday", "email" }));

            //OpenAuth.AuthenticationClients.AddMicrosoft(
            //    clientId: MicrosoftApiKey,
            //    clientSecret: MicrosoftApiSecret);

            //OpenAuth.AuthenticationClients.AddGoogle();

            //TODO - Add the DotNetOpenAuth Tables to DB
            //OpenAuth.ConnectionString = "AuthenticationRepository";
        }
    }
}