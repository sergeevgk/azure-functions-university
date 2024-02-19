using Newtonsoft.Json;
namespace AzureFunctionsUniversity_QueueBinding.Models;

public class Accomplishment
{
	[JsonProperty("title")]
	public string Title { get; set; }

	[JsonProperty("date")]
	public DateTime Date { get; set; }

	[JsonProperty("link")]
	public string Link { get; set; }
}
