using AzureFunctionsUniversity_SharedEntities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionsUniversity_QueueBinding
{
	public class NewPlayerWithStringQueueOutput
	{
		private readonly ILogger<NewPlayerWithStringQueueOutput> _logger;

		public NewPlayerWithStringQueueOutput(ILogger<NewPlayerWithStringQueueOutput> logger)
		{
			_logger = logger;
		}

		[Function(nameof(NewPlayerWithStringQueueOutput))]
		[QueueOutput(QueueConfig.NEW_PLAYER_ITEMS)]
		public async Task<string> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethod.Post))] HttpRequestData request, [FromBody] Player player)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");
			var response = JsonConvert.SerializeObject(player);
			return response;
		}
	}
}
