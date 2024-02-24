using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
[assembly: FunctionsStartup(typeof(AzureFunctionsUniversity_Configuration.Startup))]

namespace AzureFunctionsUniversity_Configuration;

/// <summary>
/// This approach does not work for isolated process azure functions. 
/// see https://learn.microsoft.com/en-us/azure/azure-app-configuration/quickstart-azure-functions-csharp?tabs=isolated-process
/// </summary>
class Startup : FunctionsStartup
{
	public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
	{
		builder.ConfigurationBuilder.AddAzureAppConfiguration(options =>
		{
			options.Connect(Environment.GetEnvironmentVariable("AppConfigurationConnectionString"));
		});
	}

	public override void Configure(IFunctionsHostBuilder builder)
	{
		builder.Services.AddAzureAppConfiguration();
	}
}