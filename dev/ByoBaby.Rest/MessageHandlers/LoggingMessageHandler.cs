using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NLog;
namespace ByoBaby.Rest.MessageHandlers
{
	public class LoggingMessageHandler : DelegatingHandler
	{
		private Logger logger = LogManager.GetLogger("ByoBaby.Rest.ApiLogging");
		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
		{
			logger.Info("Starting an API transaction");
			try
			{
				return base.SendAsync(request, cancellationToken);
			}
			catch(Exception ex)
			{
				logger.Error("Error processing API action", ex);
				throw ex;
			}
		}
	}
}