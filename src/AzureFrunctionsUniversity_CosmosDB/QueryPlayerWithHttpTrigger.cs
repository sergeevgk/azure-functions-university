using AzureFunctionsUniversity_SharedEntities;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFrunctionsUniversity_CosmosDB
{
	public class QueryPlayerWithHttpTrigger
	{
		private readonly ILogger<QueryPlayerWithHttpTrigger> _logger;

		public QueryPlayerWithHttpTrigger(ILogger<QueryPlayerWithHttpTrigger> logger)
		{
			_logger = logger;
		}

		[Function(nameof(QueryPlayerWithHttpTrigger))]
		public async Task<HttpResponseData> Run(
			[HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethod.Get), Route = "{collectionName}/{partitionKey}/{id}")] HttpRequestData request,
			[CosmosDBInput(databaseName: "Players", containerName: "{collectionName}", Id = "{id}", PartitionKey = "{partitionKey}", Connection = "CosmosDBConnection")] Player playerItem)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");
			HttpResponseData response;

			if (playerItem == null)
			{
				response = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
				await response.WriteStringAsync("Please provide ");
				return response;
			}

			response = request.CreateResponse(System.Net.HttpStatusCode.OK);
			await response.WriteAsJsonAsync(playerItem);

			return response;
		}
	}
}
