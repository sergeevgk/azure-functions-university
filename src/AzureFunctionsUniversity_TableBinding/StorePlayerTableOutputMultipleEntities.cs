using AzureFunctionsUniversity_TableBinding.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUniversity_TableBinding
{
	public class StorePlayerTableOutputMultipleEntities
	{
		private readonly ILogger<StorePlayerTableOutputMultipleEntities> _logger;

		public StorePlayerTableOutputMultipleEntities(ILogger<StorePlayerTableOutputMultipleEntities> logger)
		{
			_logger = logger;
		}

		[Function(nameof(StorePlayerTableOutputMultipleEntities))]
		[TableOutput(TableConfig.PLAYER_TABLE_NAME)]
		public PlayerEntity[] Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Post))] HttpRequestData request, [FromBody] PlayerEntity[] playerEntities)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");

			return playerEntities;
		}
	}
}
