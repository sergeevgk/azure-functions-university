using AzureFunctionsUniversity.Lesson1.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace AzureFunctionsUniversity.Lesson1
{
	public class DeveloperResumeFunction
	{
		private readonly ILogger<DeveloperResumeFunction> _logger;

		public DeveloperResumeFunction(ILogger<DeveloperResumeFunction> logger)
		{
			_logger = logger;
		}

		[Function("DeveloperResumeFunction")]
		public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData request)
		{
			_logger.LogInformation("HTTP trigger function processed a request.");

			var resume = new Resume
			{
				Name = "Georgii Sergeev",
				Website = "https://linkedin.com/in/sergeevgk",
				Country = "The Netherlands",
				Skills =
				[
					".NET", "Azure", "SQL", "JS"
				],
				CurrentRole = "Fullstack Developer at Murano Global"
			};

			var response = request.CreateResponse(HttpStatusCode.OK);
			response.WriteAsJsonAsync(resume);

			return response;
		}
	}
}
