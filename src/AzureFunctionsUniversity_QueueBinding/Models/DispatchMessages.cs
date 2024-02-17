using AzureFrunctionsUniversity_QueueBinding.Models;
using Microsoft.Azure.Functions.Worker;

namespace AzureFunctionsUniversity_QueueBinding.Models;

public class DispatchMessages
{
	[QueueOutput(QueueConfig.NEW_PLAYER_ITEMS)]
	public IEnumerable<Player> PlayerMessages { get; set; }
}
