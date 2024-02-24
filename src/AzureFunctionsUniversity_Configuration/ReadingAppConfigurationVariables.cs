using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUniversity_Configuration
{
	public class ReadingAppConfigurationVariables
	{
		private readonly ILogger<ReadingAppConfigurationVariables> _logger;
		public IConfiguration Configuration { get; }

		public ReadingAppConfigurationVariables(ILogger<ReadingAppConfigurationVariables> logger, IConfiguration configuration)
		{
			_logger = logger;
			Configuration = configuration;
		}

		[Function(nameof(ReadingAppConfigurationVariables))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, nameof(HttpMethod.Get))] HttpRequestData request)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");

			var response = request.CreateResponse(System.Net.HttpStatusCode.OK);
			var configValue = Configuration["ConfigurationValue"];
			await response.WriteStringAsync($"ConfigurationValue: {configValue}");

			return response;
		}
	}
}
