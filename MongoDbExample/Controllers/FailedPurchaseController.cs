using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;
using System.Collections.Generic;
using MongoDbExample.Entities;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FailedPurchaseController : ControllerBase
    {
        private readonly IMongoCollection<BsonDocument> _failedPurchases;

        public FailedPurchaseController(IMongoDatabase database)
        {
            _failedPurchases = database.GetCollection<BsonDocument>("FailedPurchases");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FailedPurchase failedPurchase)
        {
            if (failedPurchase == null)
            {
                return BadRequest("FailedPurchase is null.");
            }

            var document = new BsonDocument
            {
                { "_id", BsonValue.Create(failedPurchase.Id.ToString()) },
                { "CardNumber", failedPurchase.CardNumber },
                { "PurchaseDate", failedPurchase.PurchaseDate },
                { "Amount", failedPurchase.Amount },
                { "Status", failedPurchase.Status },
                { "ErrorMessage", failedPurchase.ErrorMessage },
                { "IsRetriable", failedPurchase.IsRetriable }, // <-- Incluye IsRetriable al guardar
                { "CreatedAt", failedPurchase.CreatedAt }
            };

            await _failedPurchases.InsertOneAsync(document);
            return Ok(failedPurchase);
        }

        [HttpGet]
        public async Task<IEnumerable<FailedPurchase>> Get()
        {
            var documents = await _failedPurchases.Find(_ => true).ToListAsync();
            var failedPurchases = new List<FailedPurchase>();

            foreach (var doc in documents)
            {
                failedPurchases.Add(new FailedPurchase
                {
                    Id = Guid.Parse(doc["_id"].AsString),
                    CardNumber = doc["CardNumber"].AsString,
                    PurchaseDate = doc["PurchaseDate"].ToUniversalTime(),
                    Amount = doc["Amount"].ToDecimal(),
                    Status = doc["Status"].AsString,
                    ErrorMessage = doc["ErrorMessage"].AsString,
                    IsRetriable = doc.Contains("IsRetriable") && doc["IsRetriable"].AsBoolean, // <-- Verifica IsRetriable al leer
                    CreatedAt = doc["CreatedAt"].ToUniversalTime()
                });
            }

            return failedPurchases;
        }
    }
}
