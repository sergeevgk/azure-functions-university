using AzureFunctionsUniversity_DurableFunctionsAdvanced.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUniversity_DurableFunctionsAdvanced
{
	public static class OnboardingStarter
	{
		[Function(nameof(OnboardingStarter))]
		public static async Task<string> RunOrchestrator(
			[OrchestrationTrigger] TaskOrchestrationContext context)
		{
			ILogger logger = context.CreateReplaySafeLogger(nameof(OnboardingStarter));
			var onboardingTasks = new List<Task<string>>();
			var input = context.GetInput<OnboardingEmployee>();
			logger.LogInformation($"Starting the onboarding process for [{input.Name}] process [{input.ProcessId}].");

			onboardingTasks.Add(context.CallActivityAsync<string>(nameof(AccessCardCreationActivity), input));
			onboardingTasks.Add(context.CallActivityAsync<string>(nameof(ItEquipmentOrderActivity), input));
			onboardingTasks.Add(context.CallActivityAsync<string>(nameof(WelcomeEmailActivity), input));

			var results = await Task.WhenAll(onboardingTasks);
			var resultAggregated = string.Join(",", results);

			return resultAggregated;
		}

		[Function(nameof(AccessCardCreationActivity))]
		public static string AccessCardCreationActivity([ActivityTrigger] OnboardingEmployee employee, FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger(nameof(AccessCardCreationActivity));
			var message = $"Access card created for {employee.Name} starting on {employee.StartDate}";
			logger.LogInformation(message);

			return message;
		}

		[Function(nameof(ItEquipmentOrderActivity))]
		public static string ItEquipmentOrderActivity([ActivityTrigger] OnboardingEmployee employee, FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger(nameof(ItEquipmentOrderActivity));
			var message = $"Role specific IT equipment (role: {employee.Role} was ordered for {employee.Name} starting on {employee.StartDate}.";
			logger.LogInformation(message);

			return message;
		}

		[Function(nameof(WelcomeEmailActivity))]
		public static string WelcomeEmailActivity([ActivityTrigger] OnboardingEmployee employee, FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger(nameof(WelcomeEmailActivity));
			var message = $"Welcome {employee.Name}! Happy to have you on board and see you on {employee.StartDate}!";
			logger.LogInformation($"Sending email {message} to {employee.Email}.");

			return message;
		}


		[Function("OnboardingStarter_HttpStart")]
		public static async Task<HttpResponseData> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
			[DurableClient] DurableTaskClient client,
			FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger("OnboardingStarter_HttpStart");

			// Function input comes from the request content.
			var input = await req.ReadFromJsonAsync<OnboardingEmployee>();
			string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
				nameof(OnboardingStarter), input);

			logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

			// Returns an HTTP 202 response with an instance management payload.
			// See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
			return client.CreateCheckStatusResponse(req, instanceId);
		}
	}
}
