using AzureFrunctionsUniversity_QueueBinding.Models;
using AzureFunctionsUniversity_QueueBinding.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUniversity_QueueBinding
{
	public class NewPlayerWithIAsyncCollectorQueueOutput
	{
		private readonly ILogger<NewPlayerWithIAsyncCollectorQueueOutput> _logger;

		public NewPlayerWithIAsyncCollectorQueueOutput(ILogger<NewPlayerWithIAsyncCollectorQueueOutput> logger)
		{
			_logger = logger;
		}

		[Function(nameof(NewPlayerWithIAsyncCollectorQueueOutput))]
		public DispatchMessages Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethod.Post))] HttpRequestData req, [FromBody] Player[] players)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");

		var dispatchMessages = new DispatchMessages()
		{
			PlayerMessages =  players
		};

			return dispatchMessages;
		}
	}
}
