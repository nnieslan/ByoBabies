using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.OAuth.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ByoBaby.Security
{
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Adds the value from an XDocument with the specified element name if it's not empty.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary. 
        /// </param>
        /// <param name="document">
        /// The document. 
        /// </param>
        /// <param name="elementName">
        /// Name of the element. 
        /// </param>
        public static void AddDataIfNotEmpty(this Dictionary<string, string> dictionary, XDocument document, string elementName)
        {
            XElement xElement = document.Root.Element(elementName);
            if (xElement != null)
            {
                dictionary.AddItemIfNotEmpty(elementName, xElement.Value);
            }
        }
        /// <summary>
        /// Adds a key/value pair to the specified dictionary if the value is not null or empty.
        /// </summary>
        /// <param name="dictionary">
        /// The dictionary. 
        /// </param>
        /// <param name="key">
        /// The key. 
        /// </param>
        /// <param name="value">
        /// The value. 
        /// </param>
        public static void AddItemIfNotEmpty(this IDictionary<string, string> dictionary, string key, string value)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }
            if (!string.IsNullOrEmpty(value))
            {
                dictionary[key] = value;
            }
        }
    }

    public class EbLinkedInClient : OAuthClient
    {

        /// <summary>
		/// Describes the OAuth service provider endpoints for LinkedIn.
		/// </summary>
        public static readonly ServiceProviderDescription LinkedInServiceDescription = new ServiceProviderDescription()
        {
            RequestTokenEndpoint = new MessageReceivingEndpoint("https://api.linkedin.com/uas/oauth/requestToken", (HttpDeliveryMethods)5),
            UserAuthorizationEndpoint = new MessageReceivingEndpoint("https://www.linkedin.com/uas/oauth/authenticate", (HttpDeliveryMethods)5),
            AccessTokenEndpoint = new MessageReceivingEndpoint("https://api.linkedin.com/uas/oauth/accessToken", (HttpDeliveryMethods)5),
            TamperProtectionElements = new ITamperProtectionChannelBindingElement[] { new HmacSha1SigningBindingElement() }
        };

        #region ctor
        
        /// <summary>
		/// Initializes a new instance of the <see cref="T:EgoBoom.Security.EbLinkedInClient" /> class.
		/// </summary>
		/// <remarks>
		/// Tokens exchanged during the OAuth handshake are stored in cookies.
		/// </remarks>
		/// <param name="consumerKey">
		/// The LinkedIn app's consumer key. 
		/// </param>
		/// <param name="consumerSecret">
		/// The LinkedIn app's consumer secret. 
		/// </param>
		public EbLinkedInClient(string consumerKey, string consumerSecret) 
            : this(consumerKey, consumerSecret, new CookieOAuthTokenManager())
		{
		}

		/// <summary>
        /// Initializes a new instance of the <see cref="T:EgoBoom.Security.EbLinkedInClient" /> class.
		/// </summary>
		/// <param name="consumerKey">The consumer key.</param>
		/// <param name="consumerSecret">The consumer secret.</param>
		/// <param name="tokenManager">The token manager.</param>
        public EbLinkedInClient(string consumerKey, string consumerSecret, IOAuthTokenManager tokenManager)
            : base("linkedIn", EbLinkedInClient.LinkedInServiceDescription, new SimpleConsumerTokenManager(consumerKey, consumerSecret, tokenManager))
		{
		}

        #endregion

        #region OAuthClient overrides
        
        /// <summary>
		/// Check if authentication succeeded after user is redirected back from the service provider.
		/// </summary>
		/// <param name="response">
		/// The response token returned from service provider 
		/// </param>
		/// <returns>
		/// Authentication result. 
		/// </returns>
		protected override AuthenticationResult VerifyAuthenticationCore(AuthorizedTokenResponse response)
		{
			string accessToken = response.AccessToken;
			MessageReceivingEndpoint profileEndpoint = new MessageReceivingEndpoint("https://api.linkedin.com/v1/people/~:(id,first-name,last-name,headline,industry,summary)", (HttpDeliveryMethods)4);
			HttpWebRequest httpWebRequest = base.WebWorker.PrepareAuthorizedRequest(profileEndpoint, accessToken);
			AuthenticationResult result;
			try
			{
				using (WebResponse response2 = httpWebRequest.GetResponse())
				{
					using (Stream responseStream = response2.GetResponseStream())
					{
                        XmlReaderSettings xmlReaderSettings = new XmlReaderSettings() { }; //TODO - verify the settings here
                        xmlReaderSettings.MaxCharactersInDocument = 65536L;
                        XDocument xDocument = XDocument.Load(XmlReader.Create(responseStream, xmlReaderSettings));
						
                        string value = xDocument.Root.Element("id").Value;
						string value2 = xDocument.Root.Element("first-name").Value;
						string value3 = xDocument.Root.Element("last-name").Value;
						string text = value2 + " " + value3;
						Dictionary<string, string> dictionary = new Dictionary<string, string>();
						dictionary.Add("accesstoken", accessToken);
						dictionary.Add("name", text);
						dictionary.AddDataIfNotEmpty(xDocument, "headline");
						dictionary.AddDataIfNotEmpty(xDocument, "summary");
						dictionary.AddDataIfNotEmpty(xDocument, "industry");
						result = new AuthenticationResult(true, base.ProviderName, value, text, dictionary);
					}
				}
			}
			catch (Exception exception)
			{
				result = new AuthenticationResult(exception);
			}
			return result;
		}

        #endregion

        #region ISocialOAuthClient members

        /// <summary>
        /// Gets the user's LinkedIn contact list.
        /// </summary>
        /// <param name="accessToken">The access token previously retrieved.</param>
        /// <param name="maxResults">The maximum number of entries to return. If you want to receive all of the contacts, rather than only the default maximum, you can specify a very large number here.</param>
        /// <param name="startIndex">The 1-based index of the first result to be retrieved (for paging).</param>
        /// <returns>A list of the user's follows.</returns>
        public List<dynamic> GetContacts(
            string accessToken,
            string userRowKey,
            string userId)
        {
            //var contacts = new List<UserContactDto>();
            //bool more = true;
            //int maxResults = 100;
            //int startIndex = -1;

            throw new NotImplementedException();

            //return contacts;
        }

        #endregion
    }
}
