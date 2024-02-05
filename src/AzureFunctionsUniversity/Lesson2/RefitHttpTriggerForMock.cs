using AzureFunctionsUniversity.Lesson2.Contract;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AzureFunctionsUniversity.Lesson2
{
	public class RefitHttpTriggerForMock
	{
		private readonly IHttpBinOrgApiClient _client;
		private readonly ILogger _logger;

		public RefitHttpTriggerForMock(IHttpBinOrgApiClient client, ILoggerFactory loggerFactory)
		{
			_client = client;
			_logger = loggerFactory.CreateLogger<RefitHttpTriggerForMock>();
		}

		[Function(nameof(RefitHttpTriggerForMock))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData request)
		{
			var response = request.CreateResponse(HttpStatusCode.OK);

			try
			{
				var result = await _client.PostRequestParameters(request.Body);
				await response.WriteAsJsonAsync(result);
			}
			catch (Refit.ApiException e)
			{
				response.StatusCode = e.StatusCode;
				response.Headers.Add("Content-Type", "text/plain");
				await response.WriteStringAsync(e.Message);
			}

			return response;
		}
	}
}
