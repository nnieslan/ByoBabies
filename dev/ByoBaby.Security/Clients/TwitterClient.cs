using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Net;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace ByoBaby.Security
{
    public class EbTwitterClient : OAuthClient
    {
        public static readonly string TwitterPreTokenRequestUrl = "https://api.twitter.com/oauth/request_token";
        public static readonly string TwitterAuthUrl = "https://api.twitter.com/oauth/authorize";
        public static readonly string TwitterTokenUrl = "https://api.twitter.com/oauth/access_token";
        public static readonly string TwitterApiUrl = "https://api.twitter.com/1.1/";

        /// <summary>
        /// The Consumer to use for accessing Yahoo data APIs.
        /// </summary>
        public static readonly ServiceProviderDescription ServiceDescription = new ServiceProviderDescription
        {

            RequestTokenEndpoint = new MessageReceivingEndpoint(
                TwitterPreTokenRequestUrl, HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint(
                TwitterAuthUrl, HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint(
                TwitterTokenUrl, HttpDeliveryMethods.AuthorizationHeaderRequest | HttpDeliveryMethods.GetRequest),
            TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
        };

        /// <summary>
        /// The description of Twitter's OAuth protocol URIs for use with their "Sign in with Twitter" feature.
        /// </summary>
        public static readonly ServiceProviderDescription SignInWithTwitterServiceDescription = new ServiceProviderDescription
        {
            RequestTokenEndpoint = new MessageReceivingEndpoint(TwitterPreTokenRequestUrl, HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("http://twitter.com/oauth/authenticate", HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            AccessTokenEndpoint = new MessageReceivingEndpoint(TwitterTokenUrl, HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest),
            TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() },
        };

        private const string TwitterVerifyAuthQueryUrlFormat = "{0}/users/show.xml?user_id={1}";
        private const string TwitterContactsQueryUrlFormat = "{0}followers/list.json?user_id={1}&cursor={2}&count={3}&include_entities=false";
        private const string TwitterVerifyCredsUrlFormat = "{0}account/verify_credentials.json";
        private const string TwitterPostTweetUrlFormat = "{0}statuses/update.json"; //TODO - implement GeoLocation, media and other options;
        private const string TwitterDirectMessageUrlFormat = "{0}direct_messages/new.json"; //TODO - implement GeoLocation, media and other options;

        /// <summary>
        /// The consumer used for the Sign in to Twitter feature.
        /// </summary>
        private WebConsumer signInConsumer;

        /// <summary>
        /// The lock acquired to initialize the <see cref="signInConsumer"/> field.
        /// </summary>
        private object signInConsumerInitLock = new object();

        /// <summary>
        /// Gets or sets the <see cref="IConsumerTokenManager"/> used for Twitter oauth tokens.
        /// </summary>
        private IConsumerTokenManager TokenManager { get; set; }

        #region ctor

        /// <summary>
        /// Instantiates a new <see cref="EbTwitterClient"/> instance using the current EB twitter API information.
        /// </summary>
        public EbTwitterClient(IConsumerTokenManager tokenManager)
            : base("twitter", ServiceDescription, tokenManager)
        {
            this.TokenManager = tokenManager;
        }

        #endregion


        /// <summary>
        /// Gets the consumer to use for the Sign in to Twitter feature.
        /// </summary>
        /// <value>The twitter sign in.</value>
        private WebConsumer TwitterSignIn
        {
            get
            {
                if (signInConsumer == null)
                {
                    lock (signInConsumerInitLock)
                    {
                        if (signInConsumer == null)
                        {
                            signInConsumer = new WebConsumer(SignInWithTwitterServiceDescription, this.TokenManager);
                        }
                    }
                }

                return signInConsumer;
            }
        }

        protected override AuthenticationResult VerifyAuthenticationCore(
            AuthorizedTokenResponse response)
        {
            string accessToken = response.AccessToken;
            string text = response.ExtraData["user_id"];
            string userName = response.ExtraData["screen_name"];
            Uri uri = new Uri(string.Format(
                TwitterVerifyAuthQueryUrlFormat,
                TwitterApiUrl,
                text));
            //MessagingUtilities.EscapeUriDataStringRfc3986(text)));  TODO - consider doing this later if we run into issues with spec chars and encoding.

            MessageReceivingEndpoint profileEndpoint = new MessageReceivingEndpoint(uri, HttpDeliveryMethods.GetRequest);
            HttpWebRequest authorizedRequest = base.WebWorker.PrepareAuthorizedRequest(profileEndpoint, accessToken);
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("accesstoken", accessToken);
            try
            {
                using (WebResponse authorizedResponse = authorizedRequest.GetResponse())
                {
                    using (Stream responseStream = authorizedResponse.GetResponseStream())
                    {
                        XDocument document = OAuthUtil.LoadXDocumentFromStream(responseStream);
                        dictionary.AddDataIfNotEmpty(document, "name");
                        dictionary.AddDataIfNotEmpty(document, "location");
                        dictionary.AddDataIfNotEmpty(document, "description");
                        dictionary.AddDataIfNotEmpty(document, "url");
                    }
                }
            }
            catch (Exception)
            {
            }
            return new AuthenticationResult(true, base.ProviderName, text, userName, dictionary);
        }
        /// <summary>
        /// Requests authorization from Yahoo to access data from a set of Yahoo applications.
        /// </summary>
        /// <param name="consumer">
        /// The Yahoo consumer previously constructed using <see cref="CreateWebConsumer"/>.
        /// </param>
        public static void RequestAuthorization(WebConsumer consumer)
        {
            if (consumer == null)
            {
                throw new ArgumentNullException("consumer");
            }

            Uri callback = OAuthUtil.GetCallbackUrlFromContext();
            var request = consumer.PrepareRequestUserAuthorization(callback, null, null);
            consumer.Channel.Send(request);
        }

        public void Tweet(
            string accessToken,
            string tweet)
        {
            var endpoint = new MessageReceivingEndpoint(
                   string.Format(TwitterPostTweetUrlFormat, TwitterApiUrl),
                   HttpDeliveryMethods.PostRequest | HttpDeliveryMethods.AuthorizationHeaderRequest);

            var consumer = new WebConsumer(EbTwitterClient.ServiceDescription, this.TokenManager);

            var data = new[] { MultipartPostPart.CreateFormPart("status", tweet) };
            var request = consumer.PrepareAuthorizedRequest(endpoint, accessToken);
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            JObject tweetResponse = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            //TODO - implement parsing of tweet response as necessary for storing data (not relevant currently in egoBoom)
        }

        public void DirectMessage(
            string accessToken,
            string userId,
            string message)
        {
            var endpoint = new MessageReceivingEndpoint(
                   string.Format(TwitterDirectMessageUrlFormat, TwitterApiUrl),
                   HttpDeliveryMethods.PostRequest | HttpDeliveryMethods.AuthorizationHeaderRequest);

            var consumer = new WebConsumer(EbTwitterClient.ServiceDescription, this.TokenManager);
            var data = new[] {
                MultipartPostPart.CreateFormPart("user_id", userId),
                MultipartPostPart.CreateFormPart("text", message),
            };
            var request = consumer.PrepareAuthorizedRequest(endpoint, accessToken, data);
            
            var response = request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            JObject dmResponse = (JObject)JToken.ReadFrom(new JsonTextReader(reader));

        }

        public bool VerifyAccount(string accessToken, Dictionary<string, string> accountSettings)
        {
            var endpoint = new MessageReceivingEndpoint(
                   string.Format(TwitterVerifyCredsUrlFormat, TwitterApiUrl),
                   HttpDeliveryMethods.GetRequest | HttpDeliveryMethods.AuthorizationHeaderRequest);

            var request = this.WebWorker.PrepareAuthorizedRequest(endpoint, accessToken);
            var response = request.GetResponse() as HttpWebResponse;
            var reader = new StreamReader(response.GetResponseStream());
            JObject data = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
            if (data.HasValues)
            {
                accountSettings.Add("screen_name", (string)data["screen_name"]);
                accountSettings.Add("id", (string)data["id_str"]);
                accountSettings.Add("name", (string)data["name"]);
                accountSettings.Add("profile_image_url", (string)data["profile_image_url"]);

            }
            return (response.StatusCode == HttpStatusCode.OK);
        }

        #region ISocialOAuthClient members

        /// <summary>
        /// Gets the user's Twitter follow list.
        /// </summary>
        /// <param name="consumer">The Twitter consumer.</param>
        /// <param name="accessToken">The access token previously retrieved.</param>
        /// <param name="maxResults">The maximum number of entries to return. If you want to receive all of the contacts, rather than only the default maximum, you can specify a very large number here.</param>
        /// <param name="startIndex">The 1-based index of the first result to be retrieved (for paging).</param>
        /// <returns>A list of the user's follows.</returns>
        public List<dynamic> GetContacts(
            string accessToken,
            string userRowKey,
            string userId)
        {
            var contacts = new List<dynamic>();
            bool more = true;
            int maxResults = 100;
            long cursor = -1;

            //TODO - eval lazy load of paging contacts at a later date (perf-enhancement)
            while (cursor != 0) //loop through pages fetching all of the contacts 
            {
                var endpoint = new MessageReceivingEndpoint(
                    string.Format(TwitterContactsQueryUrlFormat, TwitterApiUrl, userId, cursor, maxResults),
                    HttpDeliveryMethods.GetRequest);

                var consumer = new WebConsumer(EbTwitterClient.ServiceDescription, this.TokenManager);
                var request = consumer.PrepareAuthorizedRequest(endpoint, accessToken);

                var response = request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                JObject ids = (JObject)JToken.ReadFrom(new JsonTextReader(reader));
                if (ids.HasValues)
                {
                    var chunk = (from user in ids["users"].Children()
                                 select new 
                                 {
                                     ProviderUserName = (string)user["screen_name"],
                                     Name = (string)user["name"],
                                     ProviderContactId = (string)user["id"],
                                     ProviderId = "twitter",
                                     ProviderContactImageUrl = (string)user["profile_image_url"],
                                     ProviderContactImageUrlHttps = (string)user["profile_image_url_https"],
                                     OwnerUserRowKey = userRowKey
                                 });
                    if (chunk.Count() > 0)
                    {
                        contacts.AddRange(chunk);
                    }
                    cursor = (long)ids["next_cursor"];
                }
                else { break; }
            }

            return contacts;
        }

        #endregion

    }
}
