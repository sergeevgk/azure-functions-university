using AzureFunctionsUniversity_DurableFunctionsAdvanced.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUniversity_DurableFunctionsAdvanced
{
	public static class ParallelOnboardingOrchestrator
	{
		[Function(nameof(ParallelOnboardingOrchestrator))]
		public static async Task<List<string>> RunOrchestrator(
			[OrchestrationTrigger] TaskOrchestrationContext context)
		{
			ILogger logger = context.CreateReplaySafeLogger(nameof(ParallelOnboardingOrchestrator));
			var inputArray = context.GetInput<OnboardingEmployee[]>();
			logger.LogInformation("Processing inputArray.");

			var subOrchestratorTasks = inputArray
				.Select(employee => employee with { ProcessId = $"{context.InstanceId}:{employee.Name}" })
				.Select(employeeEntry => context.CallSubOrchestratorAsync<string>(nameof(OnboardingStarter), employeeEntry));

			var results = (await Task.WhenAll(subOrchestratorTasks)).ToList();

			return results;
		}

		[Function("ParallelOnboardingOrchestrator_HttpStart")]
		public static async Task<HttpResponseData> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
			[DurableClient] DurableTaskClient client,
			FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger("ParallelOnboardingOrchestrator_HttpStart");

			// Function input comes from the request content.
			var input = await req.ReadFromJsonAsync<OnboardingEmployee[]>();
			string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
				nameof(ParallelOnboardingOrchestrator), input);

			logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

			// Returns an HTTP 202 response with an instance management payload.
			// See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
			return client.CreateCheckStatusResponse(req, instanceId);
		}
	}
}
