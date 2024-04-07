using System.Text.Json.Serialization;

namespace AzureFunctionsUniversity_DurableFunctionsAdvanced.Models
{
	public record OnboardingEmployee
	{
		[JsonPropertyName("name")]
		public string Name { get; set; }
		[JsonPropertyName("email")]
		public string Email { get; set; }
		[JsonPropertyName("startDate")]
		public string StartDate { get; set; }
		[JsonPropertyName("role")]
		public string Role { get; set; }
		[JsonPropertyName("processId")]
		public string ProcessId { get; set; } = string.Empty;
	}
}
