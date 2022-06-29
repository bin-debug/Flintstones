using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlintstonesModels;

namespace FlintstonesFeedWorkerService
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

        public async Task AddAsync(FeedModel item)
        {
            var result = await this._container.CreateItemAsync<FeedModel>(item, new PartitionKey(item.Date));
        }
    }
}
