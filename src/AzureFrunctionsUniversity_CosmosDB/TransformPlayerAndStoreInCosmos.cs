using AzureFunctionsUniversity_SharedEntities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFrunctionsUniversity_CosmosDB
{
	public class TransformPlayerAndStoreInCosmos
	{
		private readonly ILogger<TransformPlayerAndStoreInCosmos> _logger;

		public TransformPlayerAndStoreInCosmos(ILogger<TransformPlayerAndStoreInCosmos> logger)
		{
			_logger = logger;
		}

		[Function(nameof(TransformPlayerAndStoreInCosmos))]
		[CosmosDBOutput(databaseName: "Players", containerName: "players", Connection = "CosmosDBConnection")]
		public object Run([QueueTrigger("myqueue-items", Connection = "QueueConnection")] Player message)
		{
			var content = JsonConvert.SerializeObject(message);
			_logger.LogInformation($"C# Queue trigger function processed: {content}");
			return message;
		}
	}
}
