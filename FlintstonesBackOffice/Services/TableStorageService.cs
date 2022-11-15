using Azure.Data.Tables;

namespace FlintstonesBackOffice.Services
{
    public class TableStorageService
    {
        public string TableName { get; set; }

        private readonly IConfiguration _configuration;
        public TableStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<TableClient> GetTableClient(string table = "")
        {
            string tableName;
            if (!string.IsNullOrEmpty(table))
                tableName = table;
            else
                tableName = TableName;

            var serviceClient = new TableServiceClient(_configuration["StorageConnectionString"]);
            var tableClient = serviceClient.GetTableClient(tableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }
    }
}
