using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFrunctionsUniversity_BlobBinding
{
	public class HelloWorldBlobTrigger
    {
        private readonly ILogger<HelloWorldBlobTrigger> _logger;

        public HelloWorldBlobTrigger(ILogger<HelloWorldBlobTrigger> logger)
        {
            _logger = logger;
        }

        [Function(nameof(HelloWorldBlobTrigger))]
        public async Task Run([BlobTrigger("samples-workitems/{name}", Connection = "localdevstorageconnection")] Stream stream, string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
        }
    }
}
