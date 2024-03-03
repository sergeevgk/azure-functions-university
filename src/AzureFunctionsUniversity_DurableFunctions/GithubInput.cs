using System.Text.Json.Serialization;

namespace AzureFunctionsUniversity_DurableFunctions
{
	public class GithubInput
	{
		[JsonPropertyName("repositoryName")]
		public string RepoName { get; set; }

		[JsonPropertyName("raiseException")]
		public bool? ShouldRaiseException { get; set; } = false;
	}
}
