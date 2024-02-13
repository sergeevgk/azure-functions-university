using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Text;

namespace AzureFrunctionsUniversity_BlobBinding
{
	public class GetPlayerWithStringInputDynamic
	{
		private readonly ILogger<GetPlayerWithStringInputDynamic> _logger;
		private readonly BlobContainerClient _playerContainerClient;

		public GetPlayerWithStringInputDynamic(ILogger<GetPlayerWithStringInputDynamic> logger, IAzureClientFactory<BlobServiceClient> blobClientFactory)
		{
			_logger = logger;
			_playerContainerClient = blobClientFactory.CreateClient(StorePlayerWithContainerBlobOutput.PLAYER_STORAGE_CLIENT_NAME).GetBlobContainerClient("players");
			_playerContainerClient.CreateIfNotExists();
		}

		[Function(nameof(GetPlayerWithStringInputDynamic))]
		public async Task<HttpResponseData> Run(
			[HttpTrigger(AuthorizationLevel.Function,nameof(HttpMethods.Get), Route = null)] HttpRequestData request)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");

			HttpResponseData result;
			string id = request.Query["id"];
			if (string.IsNullOrEmpty(id))
			{
				var errorMessage = "No player data in request.";
				result = request.CreateResponse(System.Net.HttpStatusCode.BadRequest);
				await result.WriteStringAsync(errorMessage);
			}
			else
			{
				var blobName = $"in/player-{id}.json";
				var existingBlobClient = _playerContainerClient.GetBlobClient(blobName);
				using var contentStream = await existingBlobClient.OpenReadAsync();
				using var reader = new StreamReader(contentStream, encoding: Encoding.UTF8);
				var content = await reader.ReadToEndAsync();
				result = request.CreateResponse(System.Net.HttpStatusCode.OK);
				// Instead of setting the header for content type explicitly, one can use WriteAsJsonAsync for JSON content.
				// However, with existing example WriteAsJsonAsync adds backslashes before double quotes like \", which ruins the JSON.
				// For the cases when raw JSON content is being read with StreamReader, it's worth adding a header but still writing content as string.
				result.Headers.Add("Content-Type", MediaTypeNames.Application.Json);
				await result.WriteStringAsync(content);
			}

			return result;
		}
	}
}
