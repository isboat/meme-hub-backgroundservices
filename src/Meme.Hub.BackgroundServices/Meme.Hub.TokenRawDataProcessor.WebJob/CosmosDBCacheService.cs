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
        public async Task AddItemToList(string item, TimeSpan expiration)
        {
            if (item == null) return;
            var entity = new MongoTokenEntity
            {
                CreatedDateTime = DateTime.UtcNow,
                Id = Guid.NewGuid().ToString("N"),
                StrValue = item,
                ExpiresOn = DateTime.UtcNow.Add(expiration),
            };

            var collection = _database.GetCollection<MongoTokenEntity>(_collectionName);
            await collection.InsertOneAsync(entity);
        }

        public async Task<List<string>> GetItemsFromList()
        {
            var items = await _database.GetCollection<MongoTokenEntity>(_collectionName).FindAsync(x => true);
            return (await items.ToListAsync()).Select(x => x.StrValue).ToList();
        }

        public async Task RemoveExpiredItemsAsync()
        {
        }

        public T? GetData<T>(string key)
        {
            var item = _database.GetCollection<MongoTokenEntity>(_collectionName).Find(x => x.Id == key).FirstOrDefault();

            if (item != null)
            {
                var value = item.StrValue;
                return JsonConvert.DeserializeObject<T>(value!);
            }
            return default;
        }
        public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
        {
            return false;
        }

        public bool RemoveData(string key)
        {
            var result = _database.GetCollection<MongoTokenEntity>(_collectionName).DeleteOne(x => x.Id == key);
            return result.IsAcknowledged;
        }
    }
}
