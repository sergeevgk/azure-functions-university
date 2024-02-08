using System.Text.Json.Serialization;
namespace AzureFunctionsUniversity.Lesson2.Contract;

public sealed class GetAccessTokenResponse
{
	[JsonPropertyName("access_token")]
	public string AccessToken { get; set; } = default!;
}