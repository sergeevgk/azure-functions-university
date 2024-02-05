using System.Text.Json.Serialization;
namespace AzureFunctionsUniversity.Lesson2.Contract;

public sealed class GetAccessTokenRequestCredentials
{
	[JsonPropertyName("client_id")]
	public string ClientId { get; set; } = default!;

	[JsonPropertyName("client_secret")]
	public string ClientSecret { get; set; } = default!;

	[JsonPropertyName("grant_type")]
	public string GrantType { get; set; } = "client_credentials";

	[JsonPropertyName("resource")]
	public string? Resource { get; set; }
}