using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureServices(services =>
	{
		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();
	})
	.ConfigureAppConfiguration(builder =>
	{
		var azureAppConfigConnection = Environment.GetEnvironmentVariable("AppConfigurationConnectionString");
		builder.AddAzureAppConfiguration(azureAppConfigConnection);
	})
	.Build();

host.Run();
