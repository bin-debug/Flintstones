namespace FlintstonesSiloAbstractions.Services
{
    public class CosmosDbService : ICosmosDbService<BetDTO>
    {

        private Container _container;
        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IEnumerable<BetDTO>> GetAllAsync(string queryString)
        {
            var query = this._container.GetItemQueryIterator<BetDTO>(new QueryDefinition(queryString));
            List<BetDTO> results = new List<BetDTO>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }

            return results;
        }

        public async Task<BetDTO> GetAsync(string id, string partitionKey)
        {
            try
            {
                ItemResponse<BetDTO> response = await this._container.ReadItemAsync<BetDTO>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
            catch (CosmosException ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public async Task<BetDTO> AddAsync(BetDTO item)
        {
            var key = item.ClientID.ToString();
            var result =  await this._container.CreateItemAsync<BetDTO>(item, new PartitionKey(item.ClientID.ToString()));
            var charge = result.RequestCharge;
            if (result.StatusCode == System.Net.HttpStatusCode.Created)
                return item;
            else
                return null;
        }

        public async Task UpdateAsync(string id, BetDTO item)
        {
            await this._container.UpsertItemAsync<BetDTO>(item, new PartitionKey(item.ClientID.ToString()));
        }
    }
}
