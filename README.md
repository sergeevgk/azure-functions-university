# azure-functions-university
Learning Azure functions from https://github.com/marcduiker/azure-functions-university


# Important Notes
Check the guide for isolated worker before creating a new function project (especially with isolated worker like .NET 8): https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows. 
It saves a lot of time because Microsoft.Azure.Functions.Worker.Extensions is the packages you should install. VS does not suggest you to, just says that it does not know the HttpTriggerAttribute...

# Everything else

1. Strange parameter behavior of HttpTrigger function:
	HttpTrigger does not allow to use Custom type with **\[HttpTrigger(...)\]** attribute.
It requires to pass an HttpRequest or HttpRequestData parameter with this **[HttpTrigger]** attribute and only then custom parameter with **[FromBody]** attribute can be passed. I didn't figure out why is that so, but in [Azure Functions University guide for .Net Core 3.1](https://github.com/marcduiker/azure-functions-university/blob/main/lessons/dotnetcore31/blob/README.md) there was a different flow **for in-process function**. Maybe that's what makes difference, but I didn't find anything in [Microsoft documentation for HttpTrigger](https://learn.microsoft.com/en-gb/azure/azure-functions/functions-bindings-http-webhook-trigger) or [Isolated Worker model](https://learn.microsoft.com/en-us/azure/azure-functions/dotnet-isolated-process-guide?tabs=windows) about it.

2. Using dynamic Blob output bindings section:
	This no longer works **for isolated worker model** as IBinder and Binder are members of Microsoft.Azure.WebJobs.
	I did not find a way to implement this part of lesson, so it can be skipped if one works with latest version of .NET, I guess.