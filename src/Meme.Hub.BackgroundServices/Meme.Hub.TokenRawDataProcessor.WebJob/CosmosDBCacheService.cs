using Meme.Domain.Models.TokenModels;
using Meme.Hub.WebJob.Common;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meme.Hub.TokenRawDataProcessor.WebJob
{
    public class CosmosDBCacheService
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly string _collectionName;

        public CosmosDBCacheService()
        {
            _client = new MongoClient(Settings.MongoConnectionString);
            _database = _client.GetDatabase(Settings.MongoDatabaseName);
            _collectionName = Settings.MongoCollectionName;
        }
        public async Task AddItemToList(TokenDataModel tokenData, TimeSpan expiration)
        {
            if (tokenData == null) return;

            var collection = _database.GetCollection<TokenDataModel>(_collectionName);
            await collection.InsertOneAsync(tokenData);
        }
    }
}
