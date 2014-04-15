using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Xml;
using System.Xml.Linq;
using DotNetOpenAuth.Messaging;
using DotNetOpenAuth.AspNet.Clients;
using DotNetOpenAuth.OAuth;
using DotNetOpenAuth.OAuth.ChannelElements;
using DotNetOpenAuth.OAuth.Messages;
using DotNetOpenAuth.OpenId.Extensions.OAuth;

namespace ByoBaby.Security
{
    /// <summary>
    /// A token manager that only retains tokens in memory. 
    /// Meant for SHORT TERM USE TOKENS ONLY.
    /// </summary>
    /// <remarks>
    /// A likely application of this class is for "Sign In With Twitter",
    /// where the user only signs in without providing any authorization to access
    /// Twitter APIs except to authenticate, since that access token is only useful once.
    /// </remarks>
    [Serializable]
    public class InMemoryTokenManager : IConsumerTokenManager
    {
        //TODO - persist tokens and secrets in SQL / Azure
        //  private Dictionary<string, string> tokensAndSecrets = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="InMemoryTokenManager"/> class.
        /// </summary>
        /// <param name="consumerKey">The consumer key.</param>
        /// <param name="consumerSecret">The consumer secret.</param>
        public InMemoryTokenManager(string consumerKey, string consumerSecret)
        {
            if (string.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentNullException("consumerKey");
            }
            this.TokensAndSecrets = new Dictionary<string, string>();
            this.ConsumerKey = consumerKey;
            this.ConsumerSecret = consumerSecret;
        }

        public InMemoryTokenManager()
        {
            this.TokensAndSecrets = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the consumer key.
        /// </summary>
        /// <value>The consumer key.</value>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// Gets the consumer secret.
        /// </summary>
        /// <value>The consumer secret.</value>
        public string ConsumerSecret { get; set; }

        public Dictionary<string, string> TokensAndSecrets { get; set; }

        #region ITokenManager Members

        /// <summary>
        /// Gets the Token Secret given a request or access token.
        /// </summary>
        /// <param name="token">The request or access token.</param>
        /// <returns>
        /// The secret associated with the given token.
        /// </returns>
        /// <exception cref="ArgumentException">Thrown if the secret cannot be found for the given token.</exception>
        public string GetTokenSecret(string token)
        {
            return this.TokensAndSecrets[token];
        }

        /// <summary>
        /// Stores a newly generated unauthorized request token, secret, and optional
        /// application-specific parameters for later recall.
        /// </summary>
        /// <param name="request">The request message that resulted in the generation of a new unauthorized request token.</param>
        /// <param name="response">The response message that includes the unauthorized request token.</param>
        /// <exception cref="ArgumentException">Thrown if the consumer key is not registered, or a required parameter was not found in the parameters collection.</exception>
        /// <remarks>
        /// Request tokens stored by this method SHOULD NOT associate any user account with this token.
        /// It usually opens up security holes in your application to do so.  Instead, you associate a user
        /// account with access tokens (not request tokens) in the <see cref="ExpireRequestTokenAndStoreNewAccessToken"/>
        /// method.
        /// </remarks>
        public void StoreNewRequestToken(UnauthorizedTokenRequest request, ITokenSecretContainingMessage response)
        {
            this.TokensAndSecrets[response.Token] = response.TokenSecret;
        }

        /// <summary>
        /// Deletes a request token and its associated secret and stores a new access token and secret.
        /// </summary>
        /// <param name="consumerKey">The Consumer that is exchanging its request token for an access token.</param>
        /// <param name="requestToken">The Consumer's request token that should be deleted/expired.</param>
        /// <param name="accessToken">The new access token that is being issued to the Consumer.</param>
        /// <param name="accessTokenSecret">The secret associated with the newly issued access token.</param>
        /// <remarks>
        /// 	<para>
        /// Any scope of granted privileges associated with the request token from the
        /// original call to <see cref="StoreNewRequestToken"/> should be carried over
        /// to the new Access Token.
        /// </para>
        /// 	<para>
        /// To associate a user account with the new access token,
        /// <see cref="System.Web.HttpContext.User">HttpContext.Current.User</see> may be
        /// useful in an ASP.NET web application within the implementation of this method.
        /// Alternatively you may store the access token here without associating with a user account,
        /// and wait until <see cref="WebConsumer.ProcessUserAuthorization()"/> or
        /// <see cref="DesktopConsumer.ProcessUserAuthorization(string, string)"/> return the access
        /// token to associate the access token with a user account at that point.
        /// </para>
        /// </remarks>
        public void ExpireRequestTokenAndStoreNewAccessToken(string consumerKey, string requestToken, string accessToken, string accessTokenSecret)
        {
            this.TokensAndSecrets.Remove(requestToken);
            this.TokensAndSecrets[accessToken] = accessTokenSecret;
        }

        /// <summary>
        /// Classifies a token as a request token or an access token.
        /// </summary>
        /// <param name="token">The token to classify.</param>
        /// <returns>Request or Access token, or invalid if the token is not recognized.</returns>
        public TokenType GetTokenType(string token)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public static class OAuthUtil
    {
        /// <summary>
        /// Pseudo-random data generator.
        /// </summary>
        internal static readonly Random NonCryptoRandomDataGenerator = new Random();

        internal static XmlReaderSettings CreateUntrustedXmlReaderSettings()
        {
            return new XmlReaderSettings
            {
                MaxCharactersFromEntities = 1024L,
                XmlResolver = null,
                DtdProcessing = DtdProcessing.Prohibit
            };
        }

        /// <summary>
        /// Helper method to load an XDocument from an input stream.
        /// </summary>
        /// <param name="stream">The input stream from which to load the document.</param>
        /// <returns>The XML document.</returns>
        internal static XDocument LoadXDocumentFromStream(Stream stream)
        {
            XmlReaderSettings xmlReaderSettings = OAuthUtil.CreateUntrustedXmlReaderSettings();
            xmlReaderSettings.MaxCharactersInDocument = 65536L;
            return XDocument.Load(XmlReader.Create(stream, xmlReaderSettings));
        }
        /// <summary>
        /// Sets the channel's outgoing HTTP requests to use default network credentials.
        /// </summary>
        /// <param name="channel">The channel to modify.</param>
        public static void UseDefaultNetworkCredentialsOnOutgoingHttpRequests(this Channel channel)
        {
            Debug.Assert(!(channel.WebRequestHandler is WrappingWebRequestHandler), "Wrapping an already wrapped web request handler.  This is legal, but highly suspect of a bug as you don't want to wrap the same channel repeatedly to apply the same effect.");
            AddOutgoingHttpRequestTransform(channel, http => http.Credentials = CredentialCache.DefaultNetworkCredentials);
        }

        /// <summary>
        /// Adds some action to any outgoing HTTP request on this channel.
        /// </summary>
        /// <param name="channel">The channel's whose outgoing HTTP requests should be modified.</param>
        /// <param name="action">The action to perform on outgoing HTTP requests.</param>
        internal static void AddOutgoingHttpRequestTransform(this Channel channel, Action<HttpWebRequest> action)
        {
            if (channel == null)
            {
                throw new ArgumentNullException("channel");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            channel.WebRequestHandler = new WrappingWebRequestHandler(channel.WebRequestHandler, action);
        }

        /// <summary>
        /// Enumerates through the individual set bits in a flag enum.
        /// </summary>
        /// <param name="flags">The flags enum value.</param>
        /// <returns>An enumeration of just the <i>set</i> bits in the flags enum.</returns>
        internal static IEnumerable<long> GetIndividualFlags(Enum flags)
        {
            long flagsLong = Convert.ToInt64(flags);
            for (int i = 0; i < sizeof(long) * 8; i++)
            { // long is the type behind the largest enum
                // Select an individual application from the scopes.
                long individualFlagPosition = (long)Math.Pow(2, i);
                long individualFlag = flagsLong & individualFlagPosition;
                if (individualFlag == individualFlagPosition)
                {
                    yield return individualFlag;
                }
            }
        }

        internal static Uri GetCallbackUrlFromContext()
        {
            Uri callback = MessagingUtilities.GetRequestUrlFromContext().StripQueryArgumentsWithPrefix("oauth_");
            return callback;
        }

        /// <summary>
        /// Copies the contents of one stream to another.
        /// </summary>
        /// <param name="copyFrom">The stream to copy from, at the position where copying should begin.</param>
        /// <param name="copyTo">The stream to copy to, at the position where bytes should be written.</param>
        /// <returns>The total number of bytes copied.</returns>
        /// <remarks>
        /// Copying begins at the streams' current positions.
        /// The positions are NOT reset after copying is complete.
        /// </remarks>
        internal static int CopyTo(this Stream copyFrom, Stream copyTo)
        {
            return CopyTo(copyFrom, copyTo, int.MaxValue);
        }

        /// <summary>
        /// Copies the contents of one stream to another.
        /// </summary>
        /// <param name="copyFrom">The stream to copy from, at the position where copying should begin.</param>
        /// <param name="copyTo">The stream to copy to, at the position where bytes should be written.</param>
        /// <param name="maximumBytesToCopy">The maximum bytes to copy.</param>
        /// <returns>The total number of bytes copied.</returns>
        /// <remarks>
        /// Copying begins at the streams' current positions.
        /// The positions are NOT reset after copying is complete.
        /// </remarks>
        internal static int CopyTo(this Stream copyFrom, Stream copyTo, int maximumBytesToCopy)
        {
            if (copyFrom == null)
            {
                throw new ArgumentNullException("copyFrom");
            }
            if (copyTo == null)
            {
                throw new ArgumentNullException("copyTo");
            }

            byte[] buffer = new byte[1024];
            int readBytes;
            int totalCopiedBytes = 0;
            while ((readBytes = copyFrom.Read(buffer, 0, Math.Min(1024, maximumBytesToCopy))) > 0)
            {
                int writeBytes = Math.Min(maximumBytesToCopy, readBytes);
                copyTo.Write(buffer, 0, writeBytes);
                totalCopiedBytes += writeBytes;
                maximumBytesToCopy -= writeBytes;
            }

            return totalCopiedBytes;
        }

        /// <summary>
        /// Wraps some instance of a web request handler in order to perform some extra operation on all
        /// outgoing HTTP requests.
        /// </summary>
        private class WrappingWebRequestHandler : IDirectWebRequestHandler
        {
            /// <summary>
            /// The handler being wrapped.
            /// </summary>
            private readonly IDirectWebRequestHandler wrappedHandler;

            /// <summary>
            /// The action to perform on outgoing HTTP requests.
            /// </summary>
            private readonly Action<HttpWebRequest> action;

            /// <summary>
            /// Initializes a new instance of the <see cref="WrappingWebRequestHandler"/> class.
            /// </summary>
            /// <param name="wrappedHandler">The HTTP handler to wrap.</param>
            /// <param name="action">The action to perform on outgoing HTTP requests.</param>
            internal WrappingWebRequestHandler(IDirectWebRequestHandler wrappedHandler, Action<HttpWebRequest> action)
            {
                if (wrappedHandler == null)
                {
                    throw new ArgumentNullException("wrappedHandler");
                }

                if (action == null)
                {
                    throw new ArgumentNullException("action");
                }

                this.wrappedHandler = wrappedHandler;
                this.action = action;
            }

            #region Implementation of IDirectWebRequestHandler

            /// <summary>
            /// Determines whether this instance can support the specified options.
            /// </summary>
            /// <param name="options">The set of options that might be given in a subsequent web request.</param>
            /// <returns>
            /// 	<c>true</c> if this instance can support the specified options; otherwise, <c>false</c>.
            /// </returns>
            public bool CanSupport(DirectWebRequestOptions options)
            {
                return this.wrappedHandler.CanSupport(options);
            }

            /// <summary>
            /// Prepares an <see cref="HttpWebRequest"/> that contains an POST entity for sending the entity.
            /// </summary>
            /// <param name="request">The <see cref="HttpWebRequest"/> that should contain the entity.</param>
            /// <returns>
            /// The stream the caller should write out the entity data to.
            /// </returns>
            /// <exception cref="ProtocolException">Thrown for any network error.</exception>
            /// <remarks>
            /// 	<para>The caller should have set the <see cref="HttpWebRequest.ContentLength"/>
            /// and any other appropriate properties <i>before</i> calling this method.
            /// Callers <i>must</i> close and dispose of the request stream when they are done
            /// writing to it to avoid taking up the connection too long and causing long waits on
            /// subsequent requests.</para>
            /// 	<para>Implementations should catch <see cref="WebException"/> and wrap it in a
            /// <see cref="ProtocolException"/> to abstract away the transport and provide
            /// a single exception type for hosts to catch.</para>
            /// </remarks>
            public Stream GetRequestStream(HttpWebRequest request)
            {
                this.action(request);
                return this.wrappedHandler.GetRequestStream(request);
            }

            /// <summary>
            /// Prepares an <see cref="HttpWebRequest"/> that contains an POST entity for sending the entity.
            /// </summary>
            /// <param name="request">The <see cref="HttpWebRequest"/> that should contain the entity.</param>
            /// <param name="options">The options to apply to this web request.</param>
            /// <returns>
            /// The stream the caller should write out the entity data to.
            /// </returns>
            /// <exception cref="ProtocolException">Thrown for any network error.</exception>
            /// <remarks>
            /// 	<para>The caller should have set the <see cref="HttpWebRequest.ContentLength"/>
            /// and any other appropriate properties <i>before</i> calling this method.
            /// Callers <i>must</i> close and dispose of the request stream when they are done
            /// writing to it to avoid taking up the connection too long and causing long waits on
            /// subsequent requests.</para>
            /// 	<para>Implementations should catch <see cref="WebException"/> and wrap it in a
            /// <see cref="ProtocolException"/> to abstract away the transport and provide
            /// a single exception type for hosts to catch.</para>
            /// </remarks>
            public Stream GetRequestStream(HttpWebRequest request, DirectWebRequestOptions options)
            {
                this.action(request);
                return this.wrappedHandler.GetRequestStream(request, options);
            }

            /// <summary>
            /// Processes an <see cref="HttpWebRequest"/> and converts the 
            /// <see cref="HttpWebResponse"/> to a <see cref="IncomingWebResponse"/> instance.
            /// </summary>
            /// <param name="request">The <see cref="HttpWebRequest"/> to handle.</param>
            /// <returns>An instance of <see cref="IncomingWebResponse"/> describing the response.</returns>
            /// <exception cref="ProtocolException">Thrown for any network error.</exception>
            /// <remarks>
            /// 	<para>Implementations should catch <see cref="WebException"/> and wrap it in a
            /// <see cref="ProtocolException"/> to abstract away the transport and provide
            /// a single exception type for hosts to catch.  The <see cref="WebException.Response"/>
            /// value, if set, should be Closed before throwing.</para>
            /// </remarks>
            public IncomingWebResponse GetResponse(HttpWebRequest request)
            {
                // If the request has an entity, the action would have already been processed in GetRequestStream.
                if (request.Method == "GET")
                {
                    this.action(request);
                }

                return this.wrappedHandler.GetResponse(request);
            }

            /// <summary>
            /// Processes an <see cref="HttpWebRequest"/> and converts the 
            /// <see cref="HttpWebResponse"/> to a <see cref="IncomingWebResponse"/> instance.
            /// </summary>
            /// <param name="request">The <see cref="HttpWebRequest"/> to handle.</param>
            /// <param name="options">The options to apply to this web request.</param>
            /// <returns>An instance of <see cref="IncomingWebResponse"/> describing the response.</returns>
            /// <exception cref="ProtocolException">Thrown for any network error.</exception>
            /// <remarks>
            /// 	<para>Implementations should catch <see cref="WebException"/> and wrap it in a
            /// <see cref="ProtocolException"/> to abstract away the transport and provide
            /// a single exception type for hosts to catch.  The <see cref="WebException.Response"/>
            /// value, if set, should be Closed before throwing.</para>
            /// </remarks>
            public IncomingWebResponse GetResponse(HttpWebRequest request, DirectWebRequestOptions options)
            {
                // If the request has an entity, the action would have already been processed in GetRequestStream.
                if (request.Method == "GET")
                {
                    this.action(request);
                }

                return this.wrappedHandler.GetResponse(request, options);
            }

            #endregion
        }
    }
}
