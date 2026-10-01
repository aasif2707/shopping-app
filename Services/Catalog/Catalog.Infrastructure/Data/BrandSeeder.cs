using Catalog.Core.Entities;
using MongoDB.Driver;
using System.Reflection;
using System.Text.Json;

namespace Catalog.Infrastructure.Data
{
    public class BrandSeeder
    {
        public static void Seed(IMongoCollection<ProductBrand> brandCollection)
        {
            bool records = brandCollection.Find(b => true).Any();
            if (!records)
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var data = File.ReadAllText(path + @"/Data/SeedData/brands.json");
                List<ProductBrand> result = JsonSerializer.Deserialize<List<ProductBrand>>(data);
                if (result?.Count > 0)
                {
                    foreach (var record in result)
                    {
                        brandCollection.InsertOne(record);
                    }
                }
            }
        }
    }
}