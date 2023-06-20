using System;
using Azure;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp
{
    public class TableStorageRow : Azure.Data.Tables.ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Text { get; set; }
    }


    public class BlobFunction
    {
        private readonly ILogger _logger;

        public BlobFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<BlobFunction>();
        }

        [TableOutput("OutputTable", Connection = "StorageConnectionString")]
        [Function("BlobFunction")]
        public TableStorageRow Run(
            [QueueTrigger("myqueue-items", Connection = "StorageConnectionString")] string myQueueItem)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            return new TableStorageRow()
            {
                PartitionKey = "queueTriggered",
                RowKey = Guid.NewGuid().ToString(),
                Text = $"Output recorded from {myQueueItem} created at: {DateTime.UtcNow}"
            };
        }
    }
}
