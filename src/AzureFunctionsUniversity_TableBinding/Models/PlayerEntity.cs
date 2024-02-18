using Azure;
using Azure.Data.Tables;

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
		public ETag ETag { get; set; }
	}
}