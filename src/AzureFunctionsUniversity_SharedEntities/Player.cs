using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace AzureFunctionsUniversity_SharedEntities;
public class Player
{
	public Player(){}

	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("nickName")]
	public string NickName { get; set; }

	[JsonProperty("playerId")]
	public int PlayerId { get; set; }

	[JsonProperty("email")]
	public string Email { get; set; }

	[JsonProperty("region")]
	public string Region { get; set; }
}