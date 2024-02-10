using Azure.Storage.Blobs;
using AzureFrunctionsUniversity_BlobBinding.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Text;

namespace AzureFrunctionsUniversity_BlobBinding
{
	public class StorePlayerWithContainerBlobOutput
	{
		public const string PlayerStorageClientName = nameof(_playerContainerClient);
		private readonly ILogger<StorePlayerWithContainerBlobOutput> _logger;
		private readonly BlobContainerClient _playerContainerClient;

		public StorePlayerWithContainerBlobOutput(ILogger<StorePlayerWithContainerBlobOutput> logger, IAzureClientFactory<BlobServiceClient> blobClientFactory)
		{
			_logger = logger;
			_playerContainerClient = blobClientFactory.CreateClient(PlayerStorageClientName).GetBlobContainerClient("players");
			_playerContainerClient.CreateIfNotExists();
		}

		[Function("StorePlayerWithContainerBlobOutput")]
		public async Task Run([HttpTrigger(AuthorizationLevel.Function,nameof(HttpMethods.Post))] HttpRequestData req, [FromBody] Player player)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");

			var result = string.Empty;
			if (player != null) 
			{
				result = JsonConvert.SerializeObject(player, Formatting.Indented);
			}
			else
			{
				_logger.LogWarning("No player data in request.");
			}

			using (MemoryStream ms = new MemoryStream())
			{
				var sw = new StreamWriter(ms, Encoding.UTF8);
				try
				{
					await sw.WriteAsync(result);
					await sw.FlushAsync();
					ms.Seek(0, SeekOrigin.Begin);
					var blobName = $"out/blob-{player.NickName}.json";
					var existingBlobClient = _playerContainerClient.GetBlobClient(blobName);
					var res = await existingBlobClient.UploadAsync(ms, overwrite: true);
				}
				finally
				{
					sw.Dispose();
				}
			}
		}
	}
}
