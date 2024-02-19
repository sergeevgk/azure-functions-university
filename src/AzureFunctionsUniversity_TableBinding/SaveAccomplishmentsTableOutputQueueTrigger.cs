using AzureFunctionsUniversity_QueueBinding.Models;
using AzureFunctionsUniversity_SharedEntities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureFunctionsUniversity_TableBinding
{
	public class SaveAccomplishmentsTableOutputQueueTrigger
	{
		private readonly ILogger<SaveAccomplishmentsTableOutputQueueTrigger> _logger;

		public SaveAccomplishmentsTableOutputQueueTrigger(ILogger<SaveAccomplishmentsTableOutputQueueTrigger> logger)
		{
			_logger = logger;
		}

		[Function(nameof(SaveAccomplishmentsTableOutputQueueTrigger))]
		[TableOutput(TableConfig.ACCOMPLISHMENTS_TABLE_NAME)]
		public AccomplishmentEntity Run([QueueTrigger(QueueConfig.ACCOMPLISHMENTS_ITEMS)] Accomplishment accomplishment)
		{
			_logger.LogInformation("C# HTTP trigger function processed a request.");
			var accomplishmentEntity = new AccomplishmentEntity
			{
				AccomplishmentType = "Certification",
				Title = accomplishment.Title,
				Date = accomplishment.Date,
				Link = accomplishment.Link
			};

			return accomplishmentEntity;
		}
	}
}
