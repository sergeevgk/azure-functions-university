using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUniversity_Configuration
{
	public class ReadingEnvironmentVariables
	{
		private readonly ILogger<ReadingEnvironmentVariables> _logger;

		public ReadingEnvironmentVariables(ILogger<ReadingEnvironmentVariables> logger)
		{
			_logger = logger;
		}

		[Function(nameof(ReadingEnvironmentVariables))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethod.Get))] HttpRequestData request)
		{
			var config = Environment.GetEnvironmentVariable("ConfigurationValue");
			var responsne = request.CreateResponse(System.Net.HttpStatusCode.OK);
			await responsne.WriteStringAsync($"ConfigurationValue: {config}");
			return responsne;
		}
	}
}
