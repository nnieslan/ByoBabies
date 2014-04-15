using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Net;
using System.Web;
using System.Xml.Linq;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ByoBaby.Security
{
    public class OutlookClient : WebServerClient
    {
        public static readonly string MicrosoftAuthUrl = "https://login.live.com/oauth20_authorize.srf";
        public static readonly string MicrosoftTokenUrl = "https://login.live.com/oauth20_token.srf";
        public static readonly List<string> MicrosoftScope = new List<string>() 
        { 
            "wl.offline_access", 
            "wl.signin", 
            "wl.basic", 
            "wl.emails", 
            "wl.contacts_emails", 
            "wl.phone_numbers", 
            "wl.contacts_birthday", 
            "wl.postal_addresses", 
            "wl.contacts_photos", 
            "wl.contacts_calendars" 
        };
        public static readonly string MicrosoftApiUrl = "https://apis.live.net/v5.0/";

        private const string OutlookContactsQueryUrlFormat = @"{0}me/contacts?access_token={1}";
        private static readonly AuthorizationServerDescription OutlookDescription = new AuthorizationServerDescription
        {
            TokenEndpoint = new Uri(MicrosoftTokenUrl),
            AuthorizationEndpoint = new Uri(MicrosoftAuthUrl),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="OutlookClient"/> class.
        /// </summary>
        public OutlookClient()
            : base(OutlookDescription)
        {
            this.ClientIdentifier = "";  //TODO - API KEY
            this.ClientCredentialApplicator = ClientCredentialApplicator.PostParameter("");  //TODO - Secret
        }

        public List<dynamic> GetContacts(string accessToken, string userRowKey, string providerUserId = null)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }
            else
            {
                var friends = new List<dynamic>();
                //bool more = true;

                var queryUrl = string.Format(
                    OutlookContactsQueryUrlFormat, 
                    MicrosoftApiUrl, 
                    Uri.EscapeDataString(accessToken));
                //TODO - implement paging in outlook contacts querying
                //while (more)
                //{
                var request = WebRequest.Create(queryUrl);
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var reader = new StreamReader(responseStream);
                        JObject value = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                        if (value.HasValues)
                        {
                            var page = value["data"].Children();
                            foreach (var entry in page)
                            {
                                string firstname = (string)entry["first_name"];
                                string lastname = (string)entry["last_name"];
                                string userId = (string)entry["user_id"];
                                string id = (string)entry["id"];
                                string name = (string)entry["name"];

                                Dictionary<string, bool> emails = new Dictionary<string, bool>();
                                var emailsArray = entry["emails"];
                                if (emailsArray != null)
                                {
                                    string preferred = (string)emailsArray["preferred"];
                                    if (!string.IsNullOrWhiteSpace(preferred))
                                    {
                                        friends.Add(new 
                                        {
                                            ProviderContactId = id,
                                            ProviderId = "microsoft",
                                            Email = preferred,
                                            Name = name,
                                            OwnerUserRowKey = userRowKey
                                        });
                                    }
                                }
                            }

                        }
                    }
                }
                //}
                return friends;
            }
        }
    }
}
