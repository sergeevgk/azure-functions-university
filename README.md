# azure-functions-university
Learning Azure functions from https://github.com/marcduiker/azure-functions-university


# Important Notes at the beginning
Check [the guide for isolated worker](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows) before creating a new function project (especially with isolated worker like .NET 8): . 
It saves a lot of time because Microsoft.Azure.Functions.Worker.Extensions is the packages you should install. VS does not suggest you to, just says that it does not know the HttpTriggerAttribute...

Check [the guide for Durable functions](https://learn.microsoft.com/en-us/azure/azure-functions/durable/quickstart-ts-vscode?pivots=nodejs-model-v4) (available for many programming languages). There are significant differences from the version used in Azure University course (at least for Azure Functions VSCode Extension).

# Other notes

## 1. HttpTrigger Function Behavior:
- **Issue:** In .NET 8 isolated worker process HttpTrigger function does not allow the use of a Custom type with `[HttpTrigger(...)]` attribute.
- **Solution:** It requires passing an HttpRequest or HttpRequestData parameter with the `[HttpTrigger]` attribute, and then a custom parameter with `[FromBody]` attribute can be passed.
- **Note:** In [Azure Functions University guide for .Net Core 3.1](https://github.com/marcduiker/azure-functions-university/blob/main/lessons/dotnetcore31/blob/README.md) the parameters of HttpTrigger were slightly different, but it was an in-process function. Maybe that's what makes difference, but I didn't find anything in [Microsoft documentation for HttpTrigger](https://learn.microsoft.com/en-gb/azure/azure-functions/functions-bindings-http-webhook-trigger) or [Isolated Worker model](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows) about it.

## 2. Blob Output Bindings and CloudBlobContainer:
- **Issue:** Dynamic Blob output bindings and CloudBlobContainer do not work for isolated worker model.
- **Solution:** It is recommended to register and use appropriate Sdk Clients (e.g., BlobServiceClient) explicitly instead.

## 3. Reference Examples for Bindings:
- **Resource:** Reference examples for all kinds of Bindings using the isolation worker Extensions Sdk.
- **Link:** [Azure Functions Worker Samples](https://github.com/Azure/azure-functions-dotnet-worker/blob/main/samples/Extensions/)
- **Note:** This was a helpful discovery for me as I struggled with questions "Am I doing this in a right way? Are there any recommended or just different options?".
Especially worth checking the [Blob section](https://github.com/Azure/azure-functions-dotnet-worker/blob/main/samples/Extensions/Blob/BlobInputBindingSamples.cs) for clearer examples. This one gave me a simple example which I was looking for the isolated worker model in Blob binding lesson.

## 4. App Configuration Service Registration:
- **Issue:** App Configuration Service registration for isolated worker no longer uses FunctionsStartup.
- **Solution:** Add configuration settings to Program.cs.
```
	.ConfigureAppConfiguration(builder =>
	{
		var azureAppConfigConnection = Environment.GetEnvironmentVariable("AppConfigurationConnectionString");
		builder.AddAzureAppConfiguration(azureAppConfigConnection);
	})
```
- **Note:** Worth checking [Documentation for adding these Configuration settings](https://learn.microsoft.com/en-us/azure/azure-app-configuration/quickstart-azure-functions-csharp?tabs=isolated-process).

## 5. Durable Functions with Isolated Process Model (.NET 8):
- **Issue:** Default example created with a new project does not work.
- **Workaround:** Add a specific configuration to Program.cs as a workaround.
```
	services.Configure<KestrelServerOptions>(options =>
	{
		options.AllowSynchronousIO = true;
	});
``` 
- **Reference:** For more options, check [this issue](https://github.com/Azure/azure-functions-durable-extension/issues/2683).
- **Note:** The problem might be related to Azure Functions Dotnet worker and eventually can be fixed there. Relevant issues:
    - https://github.com/Azure/azure-functions-dotnet-worker/issues/2227
    - https://github.com/Azure/azure-functions-dotnet-worker/issues/2127
    - https://github.com/Azure/azure-functions-dotnet-worker/issues/2205
