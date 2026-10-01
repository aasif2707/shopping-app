using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Reflection;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public class ProductSeeder
    {
        public static void Seed(IMongoCollection<Product> productCollection)
        {
            bool records = productCollection.Find(b => true).Any();
            if (!records)
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var data = File.ReadAllText(path + @"/Data/SeedData/products.json");
                List<Product> result = JsonSerializer.Deserialize<List<Product>>(data);
                if (result?.Count > 0)
                {
                    foreach (var record in result)
                    {
                        productCollection.InsertOne(record);
                    }
                }
            }
        }
    }
}