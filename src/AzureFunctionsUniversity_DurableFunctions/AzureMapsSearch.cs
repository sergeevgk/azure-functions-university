using Azure.Core.GeoJson;
using Azure.Maps.Search;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web;

namespace AzureFunctionsUniversity_DurableFunctions
{
	public class AzureMapsSearch
	{
		const int FIRST_RETRY_INTERVAL_MS = 1000;
		const int MAX_RETRY_INTERVAL_MS = 1000;
		const int RETRY_TIMEOUT_MS = 7000;
		const int MAX_NUMBER_OF_ATTEMPTS = 3;
		private readonly MapsSearchClient _client;

		public AzureMapsSearch(MapsSearchClient client)
		{
			_client = client ?? throw new ArgumentNullException(nameof(client));
		}

		[Function(nameof(AzureMapsSearch))]
		public async Task<List<string>> RunOrchestrator(
			[OrchestrationTrigger] TaskOrchestrationContext context)
		{
			ILogger logger = context.CreateReplaySafeLogger(nameof(AzureMapsSearch));
			logger.LogInformation("Saying hello.");

			var retryOptions = TaskOptions.FromRetryPolicy(new RetryPolicy(
				MAX_NUMBER_OF_ATTEMPTS,
				TimeSpan.FromMilliseconds(FIRST_RETRY_INTERVAL_MS),
				1,
				TimeSpan.FromMilliseconds(MAX_RETRY_INTERVAL_MS),
				TimeSpan.FromMilliseconds(RETRY_TIMEOUT_MS)));

			var input = context.GetInput<AzureMapSearchInput>();
			var outputs = new List<string>();
			try
			{
				// find location by address
				var location = await context.CallActivityAsync<string>(nameof(FindLocationByAddressAsync), input.SearchAddress, retryOptions);
				var positionValues = location.Trim('[', ']').Split(',').Select(double.Parse).ToArray();
				// perform local search 
				var localSearchInput = new LocalSearchInput
				{
					BusinessEntityName = input.BusinessEntityType,
					Longitude = positionValues[0],
					Latitude = positionValues[1]
				};
				var localSearchResult = await context.CallActivityAsync<IEnumerable<string>>(nameof(LocalSearchByEntityAsync), localSearchInput, retryOptions);
				outputs = localSearchResult.ToList();
			}
			catch (TaskFailedException ex)
			{
				logger.LogError(ex.Message);
				throw;
			}

			return outputs;
		}

		[Function(nameof(FindLocationByAddressAsync))]
		public async Task<string> FindLocationByAddressAsync([ActivityTrigger] string address, FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger(nameof(FindLocationByAddressAsync));
			logger.LogInformation($"Searching for {address}");
			var urlEncodedAddress = HttpUtility.UrlEncode(address);
			var searchResponse = await _client.SearchAddressAsync(urlEncodedAddress);
			
			var resultPosition =  searchResponse.Value.Results.FirstOrDefault()?.Position ?? default;
			var result = resultPosition.ToString();

			return result;
		}

		public record LocalSearchInput
		{
			public string BusinessEntityName { get; set; }
			public double Longitude { get; set; }
			public double Latitude { get; set; }
		}

		[Function(nameof(LocalSearchByEntityAsync))]
		public async Task<IEnumerable<string>> LocalSearchByEntityAsync([ActivityTrigger] LocalSearchInput input, FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger(nameof(LocalSearchByEntityAsync));
			logger.LogInformation($"Searching for {input.BusinessEntityName} in local area [{input.Longitude:G5},{input.Latitude:G5}]");

			var searchOptions = new SearchPointOfInterestOptions
			{
				Coordinates = new GeoPosition(input.Longitude, input.Latitude),
				RadiusInMeters = 1000,
				Top = 5
			};

			var urlEncodedBusinessEntityName = HttpUtility.UrlEncode(input.BusinessEntityName);
			var localSearchResult = await _client.SearchPointOfInterestAsync(urlEncodedBusinessEntityName, options: searchOptions); 
			var result = localSearchResult.Value.Results.Select(lsr => $"{lsr.PointOfInterest.Name}, {lsr.Address.Country}, {lsr.Address.FreeformAddress}").ToList();

			return result;
		}

		[Function($"{nameof(AzureMapsSearch)}_HttpStart")]
		public async Task<HttpResponseData> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData request,
			[DurableClient] DurableTaskClient client,
			FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger($"{nameof(AzureMapsSearch)}_HttpStart");

			// Function input comes from the request content.
			var queryStringCollection = HttpUtility.ParseQueryString(request.Url.Query);
			var input = new AzureMapSearchInput
			{
				SearchAddress = queryStringCollection["address"],
				BusinessEntityType = queryStringCollection["entityType"]
			};
			string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
				nameof(AzureMapsSearch), input);

			logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

			// Returns an HTTP 202 response with an instance management payload.
			// See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
			return client.CreateCheckStatusResponse(request, instanceId);
		}
	}
}
