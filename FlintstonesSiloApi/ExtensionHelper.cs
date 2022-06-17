namespace FlintstonesSiloApi
{
    public static class ExtensionHelper
    {
        public static async Task<CosmosDbService> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;
            Microsoft.Azure.Cosmos.CosmosClient client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            CosmosDbService cosmosDbService = new CosmosDbService(client, databaseName, containerName);
            //Microsoft.Azure.Cosmos.DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            //await database.Database.CreateContainerIfNotExistsAsync(containerName, "/betid");

            return cosmosDbService;
        }

        public static async Task<ServiceBusSender> InitializeServiceBusInstanceAsync(IConfigurationSection configurationSection)
        {
            string connectionString = configurationSection.GetSection("ConnectionString").Value;
            string queueName = configurationSection.GetSection("QueueName").Value;
            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queueName);
            return sender;
        }
    }
}
