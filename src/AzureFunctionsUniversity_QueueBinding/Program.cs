using AzureFunctionsUniversity_QueueBinding;
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
			clientBuilder.AddQueueServiceClient(hostContext.Configuration.GetSection("AzureWebJobsStorage"))
				.WithName(NewPlayerWithStringQueueOutputDynamicBinding.QUEUE_CLIENT_NAME);
			clientBuilder.AddBlobServiceClient(hostContext.Configuration.GetSection("AzureWebJobsStorage"))
				.WithName(HelloWorldQueueTrigger.PLAYER_STORAGE_CLIENT_NAME);
		});
	})
	.Build();

host.Run();
