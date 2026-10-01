using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Reflection;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public class TypeSeeder
    {
        public static void Seed(IMongoCollection<ProductType> typeCollection)
        {
            bool records = typeCollection.Find(b => true).Any();
            if (!records)
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var data = File.ReadAllText(path + @"/Data/SeedData/types.json");
                List<ProductType> result = JsonSerializer.Deserialize<List<ProductType>>(data);
                if (result?.Count > 0)
                {
                    foreach (var record in result)
                    {
                        typeCollection.InsertOne(record);
                    }
                }
            }
        }
    }
}