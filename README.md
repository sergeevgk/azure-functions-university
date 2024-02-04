# azure-functions-university
Learning Azure functions from https://github.com/marcduiker/azure-functions-university


# Important Notes
Check the guide for isolated worker before creating a new function project (especially with isolated worker like .NET 8): https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows. 
It saves a lot of time because Microsoft.Azure.Functions.Worker.Extensions is the packages you should install. VS does not suggest you to, just says that it does not know the HttpTriggerAttribute...

# Everything else