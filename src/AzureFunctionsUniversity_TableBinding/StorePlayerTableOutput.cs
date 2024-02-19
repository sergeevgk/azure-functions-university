using AzureFunctionsUniversity_TableBinding.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUniversity_TableBinding
{
	public class StorePlayerTableOutput
	{
		private readonly ILogger<StorePlayerTableOutput> _logger;

		public StorePlayerTableOutput(ILogger<StorePlayerTableOutput> logger)
		{
			_logger = logger;
		}

		[Function(nameof(StorePlayerTableOutput))]
		[TableOutput(TableConfig.PLAYER_TABLE_NAME)]
		public PlayerEntity Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Post))] HttpRequestData request, [FromBody] PlayerEntity playerEntity)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");

			return playerEntity;
		}
	}
}
