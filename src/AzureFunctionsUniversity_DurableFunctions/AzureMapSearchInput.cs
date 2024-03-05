using System.Text.Json.Serialization;

namespace AzureFunctionsUniversity_DurableFunctions;

public class AzureMapSearchInput
{
	[JsonPropertyName("searchAddress")]
	public string SearchAddress { get; set; }
	[JsonPropertyName("nusinessEntityType")]
	public string BusinessEntityType { get; set; }
}
