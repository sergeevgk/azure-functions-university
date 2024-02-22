using System.Text.Json;
using System.Text.Json.Serialization;
using Azure.Core.Serialization;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

var host = new HostBuilder()
	.ConfigureFunctionsWebApplication()
	.ConfigureServices(services =>
	{
		services.AddApplicationInsightsTelemetryWorkerService();
		services.ConfigureFunctionsApplicationInsights();
		services.AddSingleton(s => {
			var connectionString = Environment.GetEnvironmentVariable("CosmosDBConnection");
			if (string.IsNullOrEmpty(connectionString))
			{
				throw new InvalidOperationException(
					"Please specify a valid Cosmos DB Connection in the appSettings.json file or your Azure Functions Settings.");
			}

			return new CosmosClientBuilder(connectionString).WithConnectionModeDirect()
				.Build();
		});
	})
	//.ConfigureFunctionsWorkerDefaults((IFunctionsWorkerApplicationBuilder workerApplication) =>
	//{
	//	workerApplication.ConfigureSystemTextJson();
	//})
	.Build();

host.Run();


internal static class WorkerConfigurationExtensions
{
	/// <summary>
	/// Calling ConfigureFunctionsWorkerDefaults() configures the Functions Worker to use System.Text.Json for all JSON
	/// serialization and sets JsonSerializerOptions.PropertyNameCaseInsensitive = true;
	/// This method uses DI to modify the JsonSerializerOptions.
	/// </summary>
	public static IFunctionsWorkerApplicationBuilder ConfigureSystemTextJson(this IFunctionsWorkerApplicationBuilder builder)
	{
		builder.Services.Configure<JsonSerializerOptions>(jsonSerializerOptions =>
		{
			jsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			jsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;

			// override the default value
			jsonSerializerOptions.PropertyNameCaseInsensitive = false;
		});

		return builder;
	}

	/// <summary>
	/// The functions worker uses the Azure SDK's ObjectSerializer to abstract away all JSON serialization. This allows you to
	/// swap out the default System.Text.Json implementation for the Newtonsoft.Json implementation.
	/// To do so, add the Microsoft.Azure.Core.NewtonsoftJson nuget package and then update the WorkerOptions.Serializer property.
	/// This method updates the Serializer to use Newtonsoft.Json.
	/// </summary>
	public static IFunctionsWorkerApplicationBuilder UseNewtonsoftJson(this IFunctionsWorkerApplicationBuilder builder)
	{
		builder.Services.Configure<WorkerOptions>(workerOptions =>
		{
			var settings = NewtonsoftJsonObjectSerializer.CreateJsonSerializerSettings();
			settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
			settings.NullValueHandling = NullValueHandling.Ignore;

			workerOptions.Serializer = new NewtonsoftJsonObjectSerializer(settings);
		});

		return builder;
	}
}