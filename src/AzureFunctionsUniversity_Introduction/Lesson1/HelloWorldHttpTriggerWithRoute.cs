using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Web;

namespace AzureFunctionsUniversity.Lesson1
{
	public class HelloWorldHttpTriggerWithRoute
	{
		private readonly ILogger _logger;

		public HelloWorldHttpTriggerWithRoute(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<HelloWorldHttpTriggerWithRoute>();
		}

		[Function(nameof(HelloWorldHttpTriggerWithRoute))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethod.Get), nameof(HttpMethod.Post), Route = "HelloWorldHttpTriggerWithRoute/{greeting:alpha?}")] 
			HttpRequestData request,
			string greeting)
		{
			_logger.LogInformation("HTTP trigger function processed a request.");

			string name;
			if (request.Method.Equals(nameof(HttpMethod.Get), StringComparison.OrdinalIgnoreCase))
			{
				var queryStringCollection = HttpUtility.ParseQueryString(request.Url.Query);
				name = queryStringCollection["name"];
			}
			else
			{
				name = await request.ReadAsStringAsync();
			}

			var response = request.CreateResponse(HttpStatusCode.OK);

			if (string.IsNullOrEmpty(name))
			{
				response.StatusCode = HttpStatusCode.BadRequest;
				await response.WriteStringAsync("Please provide a value for the name query string parameter.");
			}
			else
			{
				response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
				await response.WriteStringAsync($"{greeting ?? "Hello"}, {name}!");
			}

			return response;
		}
	}
}
