using AzureFunctionsUniversity.Lesson2.Contract;
using AzureFunctionsUniversity.Lesson2.Extensions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Web;

namespace AzureFunctionsUniversity.Lesson2
{
	public class RefitHttpTrigger
	{
		private readonly ILogger<RefitHttpTrigger> _logger;
		private readonly IHttpBinOrgApiClient _client;

		public RefitHttpTrigger(ILogger<RefitHttpTrigger> logger, IHttpBinOrgApiClient client)
		{
			_logger = logger;
			_client = client;
		}

		[Function(nameof(RefitHttpTrigger))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequestData request)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");

			var response = request.CreateResponse(HttpStatusCode.OK);

			try
			{
				var queryStringCollection = HttpUtility.ParseQueryString(request.Url.Query);
				var queryStringDictionary = queryStringCollection.ToDictionary();
				var result = await _client.PostRequestParameters(request.Body, query: queryStringDictionary);
				await response.WriteAsJsonAsync(result);
			}
			catch (Refit.ApiException e)
			{
				response.StatusCode = HttpStatusCode.InternalServerError;
				response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
				await response.WriteStringAsync(e.Message);
			}

			return response;
		}
	}
}