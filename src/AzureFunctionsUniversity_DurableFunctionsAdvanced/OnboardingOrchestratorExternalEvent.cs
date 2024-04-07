using AzureFunctionsUniversity_DurableFunctionsAdvanced.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUniversity_DurableFunctionsAdvanced
{
	public static class OnboardingOrchestratorExternalEvent
	{
		const int TIMEOUT_MS = 20000;

		[Function(nameof(OnboardingOrchestratorExternalEvent))]
		public static async Task<string> RunOrchestrator(
			[OrchestrationTrigger] TaskOrchestrationContext context)
		{
			ILogger logger = context.CreateReplaySafeLogger(nameof(OnboardingOrchestratorExternalEvent));
			logger.LogInformation("OnboardingOrchestratorExternalEvent started");
			var input = context.GetInput<OnboardingEmployee>();
			var checkResult = await context.CallActivityAsync<string>(nameof(CheckItEquipmentValueByRoleActivity), input);

			bool orderApprovedResult = true;
			if (checkResult != "approved")
			{
				using var cancellationTokenSource = new CancellationTokenSource();
				var timeout = TimeSpan.FromMilliseconds(TIMEOUT_MS);
				var deadline = context.CurrentUtcDateTime.Add(timeout);
				var timeoutTask = context.CreateTimer(deadline, cancellationTokenSource.Token);
				var approvalTask = context.WaitForExternalEvent<string>("ApprovalRequest");
				var winnerTask = await Task.WhenAny(timeoutTask, approvalTask);

				if (winnerTask == approvalTask)
				{
					var approvalResult = approvalTask.Result;
					orderApprovedResult = approvalResult == "approved";
				}
				else
				{
					orderApprovedResult = false;
					logger.LogWarning($"Timeout [{TIMEOUT_MS}]ms for order approval has expired.");
				}
				if (!timeoutTask.IsCompleted)
				{
					cancellationTokenSource.Cancel();
				}
			}

			if (!orderApprovedResult)
			{
				var message = "Order was declined";
				logger.LogError(message);

				return message;
			}

			var orderResult = await context.CallActivityAsync<string>("ItEquipmentOrderActivity", input);

			return orderResult;
		}

		[Function(nameof(CheckItEquipmentValueByRoleActivity))]
		public static string CheckItEquipmentValueByRoleActivity([ActivityTrigger] OnboardingEmployee employee, FunctionContext executionContext)
		{
			string message;
			if (employee.Role == "sales")
				message = "approval needed";
			else
				message = "approved";

			return message;
		}

		[Function("OnboardingOrchestratorExternalEvent_HttpStart")]
		public static async Task<HttpResponseData> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req,
			[DurableClient] DurableTaskClient client,
			FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger("OnboardingOrchestratorExternalEvent_HttpStart");

			// Function input comes from the request content.
			var input = await req.ReadFromJsonAsync<OnboardingEmployee>();
			string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
				nameof(OnboardingOrchestratorExternalEvent), input);

			logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

			// Returns an HTTP 202 response with an instance management payload.
			// See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
			return client.CreateCheckStatusResponse(req, instanceId);
		}
	}
}
