using AzureFunctionsUniversity.Lesson1.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AzureFunctionsUniversity.Lesson1
{
	public class HelloWorldHttpTriggerWithJsonPost
	{
		private readonly ILogger _logger;

		public HelloWorldHttpTriggerWithJsonPost(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<HelloWorldHttpTriggerWithJsonPost>();
		}

		[Function(nameof(HelloWorldHttpTriggerWithJsonPost))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethod.Post))] HttpRequestData request)
		{
			_logger.LogInformation("HTTP trigger function processed a request.");

			var person = await request.ReadFromJsonAsync<Person>();

			var response = request.CreateResponse(HttpStatusCode.OK);

			if (string.IsNullOrEmpty(person.Name))
			{
				response.StatusCode = HttpStatusCode.BadRequest;
				await response.WriteStringAsync("Please provide a value for the name in JSON format in the body.");
			}
			else
			{
				response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
				await response.WriteStringAsync($"Hello, {person.Name}!");
			}


			return response;
		}
	}
}
