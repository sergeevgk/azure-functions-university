using System.Text.Json.Serialization;

namespace AzureFunctionsUniversity_DurableFunctions;

public class AzureMapSearchInput
{
	/// <summary>
	/// URL Encoded search address query.
	/// Example: 1600%20Amphitheatre%20Parkway%2C%20Mountain%20View%2C%20CA
	/// </summary>
	[JsonPropertyName("searchAddress")]
	public string SearchAddress { get; set; }

	/// <summary>
	/// URL Encoded entity name.
	/// Example:juice%20bar
	/// </summary>
	[JsonPropertyName("businessEntityType")]
	public string BusinessEntityType { get; set; }
}
