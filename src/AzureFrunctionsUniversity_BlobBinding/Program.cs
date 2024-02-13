using AzureFrunctionsUniversity_BlobBinding;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureServices((hostContext, services) =>
	{
		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();
		services.AddAzureClients(clientBuilder =>
		{
			clientBuilder.AddBlobServiceClient(hostContext.Configuration.GetSection("AzureWebJobsStorage"))
				.WithName(StorePlayerWithContainerBlobOutput.PLAYER_STORAGE_CLIENT_NAME);
			clientBuilder.AddBlobServiceClient(hostContext.Configuration.GetSection("AzureWebJobsStorage"))
				.WithName(DeveloperResumeFunctionBlob.RESUME_STORAGE_CLIENT_NAME);
		});
	})
	.Build();

host.Run();
