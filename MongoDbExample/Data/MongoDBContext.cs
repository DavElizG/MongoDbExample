using MongoDB.Driver;
using MongoDbExample.Entities;

namespace WebApplication1.Data
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<FailedPurchase> FailedPurchases => _database.GetCollection<FailedPurchase>("Errors");

    }
}
