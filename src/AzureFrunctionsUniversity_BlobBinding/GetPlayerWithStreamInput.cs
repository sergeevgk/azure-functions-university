using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net.Mime;

namespace AzureFrunctionsUniversity_BlobBinding
{
	public class GetPlayerWithStreamInput
	{
		private readonly ILogger<GetPlayerWithStreamInput> _logger;

		public GetPlayerWithStreamInput(ILogger<GetPlayerWithStreamInput> logger)
		{
			_logger = logger;
		}

		[Function(nameof(GetPlayerWithStreamInput))]
		public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function,nameof(HttpMethods.Get), Route = "GetPlayerWithStreamInput/{id}")] HttpRequestData req,
			string id,
			[BlobInput("players/in/player-{id}.json")] Stream playerStream)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");

			HttpResponseData result;
			if (id == null)
			{
				var errorMessage = "No player id provided in route.";
				_logger.LogWarning(errorMessage);
				result = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
				result.Headers.Add("ContentType", MediaTypeNames.Text.Plain);
				await result.WriteStringAsync(errorMessage);
			}
			else
			{
				using var reader = new StreamReader(playerStream);
				var content = await reader.ReadToEndAsync();
				result = req.CreateResponse(System.Net.HttpStatusCode.OK);
				result.Headers.Add("ContentType", MediaTypeNames.Application.Json);
				await result.WriteStringAsync(content);
			}

			return result;
		}
	}
}
