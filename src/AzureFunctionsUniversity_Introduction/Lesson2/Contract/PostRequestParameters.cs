using System.Text.Json.Serialization;

namespace AzureFunctionsUniversity.Lesson2.Contract
{
	public sealed class PostRequestParameters
	{
		public PostRequestParameters()
		{
			Args = new Dictionary<string, string>();
			Headers = new Dictionary<string, string>();
		}

		[JsonPropertyName("args")]
		public Dictionary<string, string> Args { get; set; }
		[JsonPropertyName("data")]
		public string Data { get; set; }
		[JsonPropertyName("headers")]
		public Dictionary<string, string> Headers { get; set; }
	}
}
