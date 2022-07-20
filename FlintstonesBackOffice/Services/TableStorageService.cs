using Azure.Data.Tables;

namespace FlintstonesBackOffice.Services
{
    public class TableStorageService<T> where T : ITableEntity
    {
        public string TableName { get; set; }

        private readonly IConfiguration _configuration;
        public TableStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<TableClient> GetTableClient()
        {
            var serviceClient = new TableServiceClient(_configuration["StorageConnectionString"]);
            var tableClient = serviceClient.GetTableClient(TableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }

        public async Task<T> UpsertEntityAsync(T entity)
        {
            var tableClient = await GetTableClient();
            await tableClient.UpsertEntityAsync(entity);
            return entity;
        }

        public async Task<TableEntity> QueryEntityAsync(string query, int? maxPerPage = null, IEnumerable<string> select = null)
        {
            var tableClient = await GetTableClient();
            var results = tableClient.QueryAsync<TableEntity>(query, maxPerPage, select);

            return results;
        }
    }
}
