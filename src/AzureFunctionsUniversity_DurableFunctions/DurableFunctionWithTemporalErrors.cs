using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Octokit;

namespace AzureFunctionsUniversity_DurableFunctions
{
	public static class DurableFunctionWithTemporalErrors
	{
		const int FIRST_RETRY_INTERVAL_MS = 1000;
		const int MAX_RETRY_INTERVAL_MS = 1000;
		const int RETRY_TIMEOUT_MS = 7000;
		const int MAX_NUMBER_OF_ATTEMPTS = 3;

		[Function(nameof(DurableFunctionWithTemporalErrors))]
		public static async Task<string> RunOrchestrator(
			[OrchestrationTrigger] TaskOrchestrationContext context)
		{
			ILogger logger = context.CreateReplaySafeLogger(nameof(DurableFunctionWithTemporalErrors));
			logger.LogInformation("Saying hello.");

			var retryOptions = TaskOptions.FromRetryPolicy(new RetryPolicy(
				MAX_NUMBER_OF_ATTEMPTS,
				TimeSpan.FromMilliseconds(FIRST_RETRY_INTERVAL_MS),
				1,
				TimeSpan.FromMilliseconds(MAX_RETRY_INTERVAL_MS),
				TimeSpan.FromMilliseconds(RETRY_TIMEOUT_MS)));

			string outputs = string.Empty;
			try
			{
				var input = context.GetInput<GithubInput>();
				if (context.IsReplaying)
				{
					input.ShouldRaiseException = false;
				}
				var userLogin = await context.CallActivityAsync<string>(nameof(GetRepositoryDetailsByNameWithTemporalErrorsAsync), input, retryOptions);

				outputs = await context.CallActivityAsync<string>(nameof(GetUserDetailsByIdWithTemporalErrorsAsync), userLogin, retryOptions);

			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				throw;
			}
			return outputs;
		}

		[Function(nameof(GetRepositoryDetailsByNameWithTemporalErrorsAsync))]
		public static async Task<string> GetRepositoryDetailsByNameWithTemporalErrorsAsync([ActivityTrigger] GithubInput input, FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger(nameof(GetRepositoryDetailsByNameWithTemporalErrorsAsync));
			logger.LogInformation($"Searching repo {input}.");

			if  (input.ShouldRaiseException.Value)
			{
				throw new ApplicationException($"{nameof(GetRepositoryDetailsByNameWithTemporalErrorsAsync)}. Error occurred");
			}

			var githubClient = new GitHubClient(new ProductHeaderValue("MyGithubInfoFunctions_AzureFunctionsUniversity"));
			var request = new SearchRepositoriesRequest(input.RepoName);
			var searchResults = await githubClient.Search.SearchRepo(request);
			var result = searchResults.Items.First(item => item.Name == input.RepoName).Owner.Login;

			return result;
		}

		[Function(nameof(GetUserDetailsByIdWithTemporalErrorsAsync))]
		public static async Task<string> GetUserDetailsByIdWithTemporalErrorsAsync([ActivityTrigger] string userLogin, FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger(nameof(GetUserDetailsByIdWithTemporalErrorsAsync));
			logger.LogInformation($"Searching user {userLogin}.");
			var githubClient = new GitHubClient(new ProductHeaderValue("MyGithubInfoFunctions_AzureFunctionsUniversity"));
			var user = await githubClient.User.Get(userLogin);
			var result = JsonConvert.SerializeObject(user, Formatting.Indented);

			return result;
		}

		[Function($"{nameof(DurableFunctionWithTemporalErrors)}_HttpStart")]
		public static async Task<HttpResponseData> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
			[DurableClient] DurableTaskClient client,
			FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger("DurableFunctionWithTemporalErrors_HttpStart");

			// Function input comes from the request content.
			var input = await req.ReadFromJsonAsync<GithubInput>();
			string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
				nameof(DurableFunctionWithTemporalErrors), input);

			logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

			// Returns an HTTP 202 response with an instance management payload.
			// See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
			return client.CreateCheckStatusResponse(req, instanceId);
		}
	}
}
