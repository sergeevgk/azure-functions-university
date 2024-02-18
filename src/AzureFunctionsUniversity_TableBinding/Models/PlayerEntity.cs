using Azure;
using Azure.Data.Tables;
using Newtonsoft.Json;

namespace AzureFunctionsUniversity_TableBinding.Models
{
	public class PlayerEntity : ITableEntity
	{
		public PlayerEntity()
		{ }

		public string Id { get; set; }
		public string NickName { get; set; }
		public string Email { get; set; }
		public string Region { get; set; }
		public string PartitionKey { get => Region; set { } }
		public string RowKey { get => Id; set { } }
		public DateTimeOffset? Timestamp { get; set; }
		/// <summary>
		/// Tried to resolve issue with json to entity conversion for TableInput ("Error converting 1 input parameters for Function ...")
		/// This did not help, found this issue on Github though https://github.com/MicrosoftDocs/azure-docs/issues/119619
		/// </summary>
		//[JsonProperty("odata.etag")]
		public ETag ETag { get; set; }
	}
}