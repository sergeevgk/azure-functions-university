using AzureFunctionsUniversity_SharedEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFrunctionsUniversity_CosmosDB
{
	public class StorePlayerWithDI
	{
		private readonly ILogger<StorePlayerWithDI> _logger;
		private readonly CosmosClient _cosmosClient;

		public StorePlayerWithDI(ILogger<StorePlayerWithDI> logger, CosmosClient cosmosClient)
		{
			_logger = logger;
			_cosmosClient = cosmosClient;
		}

		[Function(nameof(StorePlayerWithDI))]
		public async Task<HttpResponseData> Run(
			[HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethod.Post))] HttpRequestData request)
		{
			var myItem = await request.ReadAsStringAsync();
			HttpResponseData response;
			Player player = JsonConvert.DeserializeObject<Player>(myItem);
			player.NickName = player.NickName.ToUpperInvariant();
			//player.Id = Guid.NewGuid().ToString();

			/* Add any validations here */
			var container = _cosmosClient.GetContainer("Players", "players");

			try
			{
				ItemResponse<Player> item = await container.UpsertItemAsync(player, new PartitionKey(player.Region));
				//var serviceResponse = await container.UpsertItemAsync(new { id = "10", player.NickName });
				response = request.CreateResponse(System.Net.HttpStatusCode.OK);
				await response.WriteAsJsonAsync(item);
				return response;
			}
			catch (CosmosException ex)
			{
				response = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
				await response.WriteAsJsonAsync(ex.Message);
				return response;
			}
		}
	}
}
