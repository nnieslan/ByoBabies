using System.Net.Http;
using System.Web;
using DotNetOpenAuth.Messaging;

namespace ByoBaby.Rest.Infrastructure
{
    public static class HttpRequestMessageExtensions
    {
        const string HttpContextKey = "MS_HttpContext";
        public static bool TryGetHttpContext(
            this HttpRequestMessage requestMessage, 
            out HttpContextBase httpContext)
        {
            httpContext = null;
            object obj;
            if (requestMessage.Properties.TryGetValue(HttpContextKey, out obj))
                httpContext = (HttpContextBase)obj;

            return httpContext != null;
        }

        public static HttpResponseMessage ToHttpResponseMessage(this OutgoingWebResponse webResponse)
        {
            // copy status code...
            var responseMsg = new HttpResponseMessage(webResponse.Status);

            // copy headers...
            foreach (string name in webResponse.Headers.Keys)
                responseMsg.Headers.Add(name, webResponse.Headers[name]);

            // copy response stream...
            if (webResponse.ResponseStream != null)
                responseMsg.Content = new StreamContent(webResponse.ResponseStream);

            return responseMsg;
        }
    }
}