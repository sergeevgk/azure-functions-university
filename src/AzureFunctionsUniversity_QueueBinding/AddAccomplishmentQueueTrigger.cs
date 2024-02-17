using Azure.Storage.Blobs;
using Azure.Storage.Queues.Models;
using AzureFunctionsUniversity_QueueBinding.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace AzureFunctionsUniversity_QueueBinding
{
	public class AddAccomplishmentQueueTrigger
    {
        private readonly ILogger<AddAccomplishmentQueueTrigger> _logger;
		public const string ACCOMP_STORAGE_CLIENT_NAME = nameof(_accompContainerClient);
		private readonly BlobContainerClient _accompContainerClient;

		public AddAccomplishmentQueueTrigger(ILogger<AddAccomplishmentQueueTrigger> logger, IAzureClientFactory<BlobServiceClient> blobClientFactory)
        {
            _logger = logger;
			_accompContainerClient = blobClientFactory.CreateClient(ACCOMP_STORAGE_CLIENT_NAME).GetBlobContainerClient("accomplishments");
			_accompContainerClient.CreateIfNotExists();
		}

        [Function(nameof(AddAccomplishmentQueueTrigger))]
        public async Task Run([QueueTrigger("accomplishment-items", Connection = "devqueueconnection")] QueueMessage message)
        {
			var messageContent = message.MessageText;
			_logger.LogInformation($"C# Queue trigger function processed: {messageContent}");
			using var ms = new MemoryStream();
			using var sw = new StreamWriter(ms, Encoding.UTF8);
			var accomplishment = JsonConvert.DeserializeObject<Accomplishment>(messageContent);
			await sw.WriteAsync(messageContent);
			await sw.FlushAsync();
			ms.Seek(0, SeekOrigin.Begin);
			var blobName = $"accomplishment-{accomplishment.Title}-{accomplishment.Date.Year}.json";
			var existingBlobClient = _accompContainerClient.GetBlobClient(blobName);
			await existingBlobClient.UploadAsync(ms, overwrite: true);
		}
    }
}
