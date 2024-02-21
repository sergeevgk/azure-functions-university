using System.Text.Json.Serialization;

namespace AzureFunctionsUniversity_SharedEntities;
public class Player
{
	public Player(){}

	[JsonPropertyName("id")]
	public string Id { get; set; }

	[JsonPropertyName("nickName")]
	public string NickName { get; set; }

	[JsonPropertyName("playerId")]
	public int PlayerId { get; set; }

	[JsonPropertyName("email")]
	public string Email { get; set; }

	[JsonPropertyName("region")]
	public string Region { get; set; }
}