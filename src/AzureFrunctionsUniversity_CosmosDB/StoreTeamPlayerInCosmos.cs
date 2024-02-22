using AzureFunctionsUniversity_SharedEntities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFrunctionsUniversity_CosmosDB
{
	public class StoreTeamPlayerInCosmos
	{
		private readonly ILogger _logger;

		public StoreTeamPlayerInCosmos(ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<StoreTeamPlayerInCosmos>();
		}

		[Function("StoreTeamPlayerInCosmos")]
		[CosmosDBOutput(databaseName: "Players", containerName: "teamplayers", Connection = "CosmosDBConnection", CreateIfNotExists = true)]
		public async Task<object> Run([CosmosDBTrigger(
			databaseName: "Players",
			containerName: "players",
			Connection = "CosmosDBConnection",
			LeaseContainerName = "leases",
			CreateLeaseContainerIfNotExists = true)] IReadOnlyList<Player> input)
		{
			var teamPlayersList = new List<TeamPlayer>();
			if (input != null && input.Count > 0)
			{
				_logger.LogInformation("Documents modified: " + input.Count);
				_logger.LogInformation("First document Id: " + input[0].Id);
				foreach (var player in input)
				{
					TeamPlayer teamPlayer = new()
					{
						Id = player.Id,
						PlayerName = player.NickName,
						Region = player.Region,
						TeamId = 1
					};
					teamPlayersList.Add(teamPlayer);
				}
			}
			return teamPlayersList;
		}
	}
}
