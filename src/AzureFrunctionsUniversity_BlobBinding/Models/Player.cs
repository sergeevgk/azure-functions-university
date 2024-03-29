﻿using Newtonsoft.Json;

namespace AzureFrunctionsUniversity_BlobBinding.Models;
public record Player
{
	public Player(){}

	[JsonProperty("id")]
	public string Id { get; set; }

	[JsonProperty("nickName")]
	public string NickName { get; set; }

	[JsonProperty("email")]
	public string Email { get; set; }

	[JsonProperty("region")]
	public string Region { get; set; }
}