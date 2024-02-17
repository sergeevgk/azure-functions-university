using Azure.Storage.Blobs;
using Azure.Storage.Queues.Models;
using AzureFrunctionsUniversity_QueueBinding.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace AzureFunctionsUniversity_QueueBinding
{
	public class HelloWorldQueueTrigger
	{
		private readonly ILogger<HelloWorldQueueTrigger> _logger;
		public const string PLAYER_STORAGE_CLIENT_NAME = nameof(_playerContainerClient);
		private readonly BlobContainerClient _playerContainerClient;

		public HelloWorldQueueTrigger(ILogger<HelloWorldQueueTrigger> logger, IAzureClientFactory<BlobServiceClient> blobClientFactory)
		{
			_logger = logger;
			_playerContainerClient = blobClientFactory.CreateClient(PLAYER_STORAGE_CLIENT_NAME).GetBlobContainerClient("players");
			_playerContainerClient.CreateIfNotExists();
		}

		[Function(nameof(HelloWorldQueueTrigger))]
		public async Task Run([QueueTrigger("myqueue-items", Connection = "devqueueconnection")] QueueMessage message)
		{
			var messageContent = message.MessageText;
			_logger.LogInformation($"C# Queue trigger function processed: {messageContent}");
			using var ms = new MemoryStream();
			using var sw = new StreamWriter(ms, Encoding.UTF8);
			var player = JsonConvert.DeserializeObject<Player>(messageContent);
			await sw.WriteAsync(messageContent);
			await sw.FlushAsync();
			ms.Seek(0, SeekOrigin.Begin);
			var blobName = $"player-{player.Id}.json";
			var existingBlobClient = _playerContainerClient.GetBlobClient(blobName);
			await existingBlobClient.UploadAsync(ms, overwrite: true);
		}
	}
}
