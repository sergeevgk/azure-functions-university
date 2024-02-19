using Azure.Data.Tables;
using AzureFunctionsUniversity_TableBinding.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUniversity_TableBinding
{
	public class GetMultiplePlayersTableClient
	{
		private readonly ILogger<GetMultiplePlayersTableClient> _logger;

		public GetMultiplePlayersTableClient(ILogger<GetMultiplePlayersTableClient> logger)
		{
			_logger = logger;
		}

		[Function(nameof(GetMultiplePlayersTableClient))]
		public async Task<HttpResponseData> Run(
			[HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Get))] HttpRequestData request, 
			[TableInput(TableConfig.TABLE_NAME)] TableClient table)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");
			string region = request.Query["region"];
			string nickName = request.Query["nickName"];

			var regionAndNickNameFilterPlayers = table.QueryAsync<PlayerEntity>(e => e.PartitionKey.Equals(region) && e.NickName.Equals(nickName));
			var playerEntitiesResult = await regionAndNickNameFilterPlayers.ToArrayAsync();

			var response = request.CreateResponse(System.Net.HttpStatusCode.OK);
			await response.WriteAsJsonAsync(playerEntitiesResult);

			return response;
		}
	}
}
