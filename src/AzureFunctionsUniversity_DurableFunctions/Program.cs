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
