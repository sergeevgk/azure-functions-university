using AzureFrunctionsUniversity_BlobBinding.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFrunctionsUniversity_BlobBinding
{
	public class StorePlayerWithStringBlobOutput
	{
		private readonly ILogger<StorePlayerWithStringBlobOutput> _logger;

		public StorePlayerWithStringBlobOutput(ILogger<StorePlayerWithStringBlobOutput> logger)
		{
			_logger = logger;
		}

		[Function("StorePlayerWithStringBlobOutput")]
		[BlobOutput("players/out/string-{rand-guid}.json")]
		public async Task<string> Run([HttpTrigger(AuthorizationLevel.Function,nameof(HttpMethods.Post))] HttpRequestData req,  [FromBody] Player player)
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

			return result;
		}
	}
}
