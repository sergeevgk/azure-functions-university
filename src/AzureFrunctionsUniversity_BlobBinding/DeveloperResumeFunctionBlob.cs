using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using System.Net.Mime;
using System.Text;

namespace AzureFrunctionsUniversity_BlobBinding
{
	public class DeveloperResumeFunctionBlob
	{
		public const string RESUME_STORAGE_CLIENT_NAME = nameof(_resumeContainerClient);
		private readonly BlobContainerClient _resumeContainerClient;

		private readonly ILogger<DeveloperResumeFunctionBlob> _logger;

		public DeveloperResumeFunctionBlob(ILogger<DeveloperResumeFunctionBlob> logger, IAzureClientFactory<BlobServiceClient> blobClientFactory)
		{
			_logger = logger;
			_resumeContainerClient = blobClientFactory.CreateClient(RESUME_STORAGE_CLIENT_NAME).GetBlobContainerClient("samples-workitems");
			_resumeContainerClient.CreateIfNotExists();
		}

		[Function(nameof(DeveloperResumeFunctionBlob))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData request)
		{
			_logger.LogInformation("HTTP trigger function processed a request.");

			var blobName = $"resume.json";
			var existingBlobClient = _resumeContainerClient.GetBlobClient(blobName);
			using var contentStream = await existingBlobClient.OpenReadAsync();
			using var reader = new StreamReader(contentStream, encoding: Encoding.UTF8);
			var content = await reader.ReadToEndAsync();
			var result = request.CreateResponse(System.Net.HttpStatusCode.OK);
			result.Headers.Add("Content-Type", MediaTypeNames.Application.Json);
			await result.WriteStringAsync(content);

			return result;
		}
	}
}