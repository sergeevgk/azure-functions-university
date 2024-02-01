using System.Net;
using System.Web;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUniversity
{
	public class HelloWorldHttpTrigger
	{
		private readonly ILogger _logger;

		public HelloWorldHttpTrigger(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<HelloWorldHttpTrigger>();
		}

		[Function("HelloWorldHttpTrigger")]
		public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethod.Get))] HttpRequestData request)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");

			var queryStringCollection = HttpUtility.ParseQueryString(request.Url.Query);
			var name = queryStringCollection["name"];

			var response = request.CreateResponse(HttpStatusCode.OK);
			response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

			response.WriteString($"Hello, {name}!");

			return response;
		}
	}
}
