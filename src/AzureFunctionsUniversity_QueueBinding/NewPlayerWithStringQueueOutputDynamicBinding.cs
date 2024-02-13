using Azure.Storage.Queues;
using AzureFrunctionsUniversity_QueueBinding.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionsUniversity_QueueBinding
{
	public class NewPlayerWithStringQueueOutputDynamicBinding
	{
		public const string QUEUE_CLIENT_NAME = nameof(_newPlayerItemsQueueClient);
		private readonly ILogger<NewPlayerWithStringQueueOutputDynamicBinding> _logger;
		private readonly QueueClient _newPlayerItemsQueueClient;

		public NewPlayerWithStringQueueOutputDynamicBinding(
			ILogger<NewPlayerWithStringQueueOutputDynamicBinding> logger,
			IAzureClientFactory<QueueServiceClient> queueClientFactory)
		{
			_logger = logger;
			_newPlayerItemsQueueClient = queueClientFactory.CreateClient(QUEUE_CLIENT_NAME).GetQueueClient(QueueConfig.NEW_PLAYER_ITEMS);
			_newPlayerItemsQueueClient.CreateIfNotExists();
		}

		[Function(nameof(NewPlayerWithStringQueueOutputDynamicBinding))]
		public async Task<HttpResponseData> Run(
			[HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethod.Post))] HttpRequestData request, 
			[FromBody] Player player)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");
			HttpResponseData result;
			if (string.IsNullOrEmpty(player.Id))
			{
				var errorMessage = "No player data provided in request.";
				_logger.LogWarning(errorMessage);
				result = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
				await result.WriteStringAsync(errorMessage);
			}
			else
			{
				var message = JsonConvert.SerializeObject(player);
				_logger.LogInformation(message);
				var sendResult = await _newPlayerItemsQueueClient.SendMessageAsync(message);
				result = request.CreateResponse(System.Net.HttpStatusCode.OK);
				await result.WriteStringAsync($"message {sendResult.Value.MessageId} was sent at {sendResult.Value.InsertionTime:u}");
			}

			return result;
		}
	}
}
