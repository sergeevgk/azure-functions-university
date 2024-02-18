using Azure.Data.Tables;
using AzureFunctionsUniversity_TableBinding.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunctionsUniversity_TableBinding
{
	public class GetPlayerByRegionAndIdCloudTableInput
	{
		private readonly ILogger<GetPlayerByRegionAndIdCloudTableInput> _logger;

		public GetPlayerByRegionAndIdCloudTableInput(ILogger<GetPlayerByRegionAndIdCloudTableInput> logger)
		{
			_logger = logger;
		}

		[Function(nameof(GetPlayerByRegionAndIdCloudTableInput))]
		public HttpResponseData Run(
			[HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethods.Get), Route = "GetPlayerByRegionAndIdCloudTableInput/{region}/{id}")] HttpRequestData request,
			[TableInput(TableConfig.TABLE_NAME, partitionKey: "{region}", rowKey: "{id}")] TableEntity playerEntity)
		{
			var playerEntityContent = JsonConvert.SerializeObject(playerEntity);
			_logger.LogInformation($"C# HTTP trigger function processed a request. Entity found: {playerEntityContent}");

			var response = request.CreateResponse(System.Net.HttpStatusCode.OK);
			response.Headers.Add("Content-Type", "application/json");
			response.WriteStringAsync(playerEntityContent);

			return response;
		}
	}
}
