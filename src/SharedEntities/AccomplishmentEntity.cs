using Azure;
using Azure.Data.Tables;

namespace AzureFunctionsUniversity_SharedEntities;

public class AccomplishmentEntity : ITableEntity
{
    public string AccomplishmentType { get; set; }
    public string Title { get; set; }
    public DateTime Date { get; set; }
    public string Link { get; set; }
	public string PartitionKey { get => AccomplishmentType; set { } }
	public string RowKey { get => Title; set { } }
	public DateTimeOffset? Timestamp { get; set; }
	public ETag ETag { get; set; }
}
