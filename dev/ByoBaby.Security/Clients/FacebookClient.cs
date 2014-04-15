using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ByoBaby.Security
{    
    public class FacebookClient : WebServerClient
    {
        public static readonly string FacebookAuthUrl = "https://www.facebook.com/dialog/oauth";
        public static readonly string FacebookTokenUrl = "https://graph.facebook.com/oauth/access_token";
        public static readonly List<string> FacebookScope = new List<string>() { "email" };//,read_stream,publish_stream";
        public static readonly string FacebookApiUrl = "https://graph.facebook.com/";

        private static readonly AuthorizationServerDescription FacebookDescription = new AuthorizationServerDescription
        {
            TokenEndpoint = new Uri(FacebookTokenUrl),
            AuthorizationEndpoint = new Uri(FacebookAuthUrl),
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="FacebookClient"/> class.
        /// </summary>
        public FacebookClient()
            : base(FacebookDescription)
        {
            this.ClientIdentifier = "";//TODO - Get API KEY/SECRET
            this.ClientCredentialApplicator = ClientCredentialApplicator.PostParameter("");//TODO - Get API KEY/SECRET
		}

        public FacebookGraph GetUserGraph(string accessToken)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ArgumentNullException("accessToken");
            }
            else
            {
                var request = WebRequest.Create("https://graph.facebook.com/me?access_token=" + Uri.EscapeDataString(accessToken));
                using (var response = request.GetResponse())
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        var graph = FacebookGraph.Deserialize(responseStream);
                        return graph;
                    }
                }
            }
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
                bool more = true;
                var friendQueryUrl = "https://graph.facebook.com/me/friends?access_token=" + Uri.EscapeDataString(accessToken);
                while (more)
                {
                    var request = WebRequest.Create(friendQueryUrl);
                    using (var response = request.GetResponse())
                    {
                        using (var responseStream = response.GetResponseStream())
                        {
                            var reader = new StreamReader(responseStream);
                            JObject data = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                            if (data.HasValues)
                            {
                                var friendChunk = (from f in data["data"].Children()
                                                   select new 
                                                   { 
                                                       ProviderContactId = (string)f["id"],
                                                       ProviderId = "facebook",
                                                       Name = (string)f["name"],
                                                       ProviderContactImageUrl = string.Format("//graph.facebook.com/{0}/picture?type=large", (string)f["id"]),
                                                       ProviderContactImageUrlHttps = string.Format("//graph.facebook.com/{0}/picture?type=large", (string)f["id"]),
                                                       OwnerUserRowKey = userRowKey
                                                   });
                                if (friendChunk.Count() > 0)
                                {
                                    friends.AddRange(friendChunk);
                                }
                                else 
                                { 
                                    more = false; 
                                }

                                var paging = data["paging"];
                                if (paging == null) { more = false; }
                                friendQueryUrl = (string)paging["next"];
                            }
                        }
                    }
                }
                return friends;
            }
        }
    }
}
