using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlintstonesResultsConsumer
{
    public class CosmosDbService
    {
        private Container _container;
        public CosmosDbService(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            this._container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task UpdateAsync(string id, BetDTO item)
        {
            await this._container.UpsertItemAsync<BetDTO>(item, new PartitionKey(item.ClientID.ToString()));
        }
    }
}
