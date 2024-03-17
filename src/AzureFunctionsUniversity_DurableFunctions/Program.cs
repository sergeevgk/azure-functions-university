using Azure.Maps.Search;
using Azure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureServices(services =>
	{
		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();
		services.AddSingleton(s =>
		{
			var apiKey = Environment.GetEnvironmentVariable("AzureMapsKey");
			if (string.IsNullOrEmpty(apiKey))
			{
				throw new InvalidOperationException(
					"Please specify a valid Azure Maps API key in the appSettings.json file or your Azure Functions Settings.");
			}
			AzureKeyCredential credential = new AzureKeyCredential(apiKey);
			MapsSearchClient client = new MapsSearchClient(credential);
			
			return client;
		});

		// this is necessary workaround to work with default example of Durable functions
		// DurableTaskClient.CreateCheckStatusResponse (both extension and class method) does a synchronous serialization call which is not allowed by default in ASP.NET Core.
		//https://github.com/Azure/azure-functions-durable-extension/issues/2683
		services.Configure<KestrelServerOptions>(options =>
		{
			options.AllowSynchronousIO = true;
		});
	})
	.Build();

host.Run();
