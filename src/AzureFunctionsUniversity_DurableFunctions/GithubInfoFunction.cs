using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Octokit;
using System.Text.Json.Serialization;

namespace AzureFunctionsUniversity_DurableFunctions
{
	public static class GithubInfoFunction
	{
		[Function(nameof(GithubInfoFunction))]
		public static async Task<string> RunOrchestrator(
			[OrchestrationTrigger] TaskOrchestrationContext context)
		{
			ILogger logger = context.CreateReplaySafeLogger(nameof(GithubInfoFunction));
			logger.LogInformation("Saying hello.");

			var input = context.GetInput<GithubInput>();
			var userLogin = await context.CallActivityAsync<string>(nameof(GetRepositoryDetailsByNameAsync), input.RepoName);

			var outputs = await context.CallActivityAsync<string>(nameof(GetUserDetailsByIdAsync), userLogin);

			return outputs;
		}

		[Function(nameof(GetRepositoryDetailsByNameAsync))]
		public static async Task<string> GetRepositoryDetailsByNameAsync([ActivityTrigger] string repoName, FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger("GetRepositoryDetailsByNameAsync");
			logger.LogInformation($"Searching repo {repoName}.");
			var githubClient = new GitHubClient(new ProductHeaderValue("MyGithubInfoFunctions_AzureFunctionsUniversity"));
			var request = new SearchRepositoriesRequest(repoName);
			var searchResults = await githubClient.Search.SearchRepo(request);
			var result = searchResults.Items.First(item => item.Name.Equals(repoName)).Owner.Login;

			return result;
		}

		[Function(nameof(GetUserDetailsByIdAsync))]
		public static async Task<string> GetUserDetailsByIdAsync([ActivityTrigger] string userLogin, FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger("GetUserDetailsByIdAsync");
			logger.LogInformation($"Searching user {userLogin}.");
			var githubClient = new GitHubClient(new ProductHeaderValue("MyGithubInfoFunctions_AzureFunctionsUniversity"));
			var user = await githubClient.User.Get(userLogin);
			var result = JsonConvert.SerializeObject(user, Formatting.Indented);

			return result;
		}

		[Function("GithubInfoFunction_HttpStart")]
		public static async Task<HttpResponseData> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req,
			[DurableClient] DurableTaskClient client,
			FunctionContext executionContext)
		{
			ILogger logger = executionContext.GetLogger("GithubInfoFunction_HttpStart");

			// Function input comes from the request content.
			var input = await req.ReadFromJsonAsync<GithubInput>();
			string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(
				nameof(GithubInfoFunction), input);

			logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);

			// Returns an HTTP 202 response with an instance management payload.
			// See https://learn.microsoft.com/azure/azure-functions/durable/durable-functions-http-api#start-orchestration
			return client.CreateCheckStatusResponse(req, instanceId);
		}
	}

	internal class GithubInput
	{
		[JsonPropertyName("repositoryName")]
		public string RepoName { get; set; }
	}
}
