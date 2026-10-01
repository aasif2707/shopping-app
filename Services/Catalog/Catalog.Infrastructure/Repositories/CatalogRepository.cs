using Catalog.Core;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Catalog.Infrastructure.Repositories
{
    public class CatalogRepository : IProductRepository, ITypeRepository, IBrandRepository
    {
        private readonly ICatalogContext _context;
        private readonly ILogger<CatalogRepository> _logger;
        public CatalogRepository(ICatalogContext context,ILogger<CatalogRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<Product> CreateProduct(Product product)
        {
            try
            {
                await _context.Products.InsertOneAsync(product);
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inside CreateProduct(). Ex: {ex.InnerException}");
                throw ex;
            }
        }
        public async Task<ProductType> CreateProductType(ProductType productType)
        {
            await _context.Types.InsertOneAsync(productType);
            return productType;
        }
        public async Task<bool> DeleteProduct(string id)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.Id, id);
                var status = await _context.Products.DeleteOneAsync(filter);
                return status.IsAcknowledged && status.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inside DeleteProduct(). Ex: {ex.InnerException}");
                throw ex;
            }
        }
        public async Task<bool> DeleteProductType(string id)
        {
            var filter = Builders<ProductType>.Filter.Eq(p => p.Id, id);
            var status = await _context.Types.DeleteOneAsync(filter);
            return status.IsAcknowledged && status?.DeletedCount > 0;
        }
        public async Task<IEnumerable<ProductBrand>> GetBrands()
        {
            try
            {
                return await _context.Brands.Find(p => true).ToListAsync();    
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Error inside GetBrands(). Ex: {ex.InnerException}");
                throw ex;
            }
        }
        public async Task<IEnumerable<Product>> GetProductByBrand(string name)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.Brands.Name, name);
                return await _context.Products.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inside GetProductByBrand(). Ex: {ex.InnerException}");
                throw ex;
            }
        }
        public async Task<Product> GetProductById(string id)
        {
            try
            {
                return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inside GetProductById(). Ex: {ex.InnerException}");
                throw ex;
            }
        }
        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.Name, name);
                return await _context.Products.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inside GetProductByName(). Ex: {ex.InnerException}");
                throw ex;
            }
        }
        public async Task<IEnumerable<Product>> GetProductByType(string name)
        {
            try
            {
                var filter = Builders<Product>.Filter.Eq(p => p.Types.Name, name);
                return await _context.Products.Find(filter).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inside GetProductByType(). Ex: {ex.InnerException}");
                throw ex;
            }
        }
        public async Task<Pagination<Product>> GetProducts(CatalogParams searchParam)
        {
            try
            {
                var builder = Builders<Product>.Filter;
                var filter = builder.Empty;
                if (!string.IsNullOrEmpty(searchParam.Search))
                {
                    var searchFilter = builder.Regex(x => x.Name, new BsonRegularExpression(searchParam.Search));
                    filter &= searchFilter;
                }
                if (!string.IsNullOrEmpty(searchParam.BrandId))
                {
                    var brandFilter = builder.Eq(x => x.Brands.Id, searchParam.BrandId);
                    filter &= brandFilter;
                }
                if (!string.IsNullOrEmpty(searchParam.TypeId))
                {
                    var typeFilter = builder.Eq(x => x.Types.Id, searchParam.TypeId);
                    filter &= typeFilter;
                }
                return new Pagination<Product>
                {   
                    PageSize = searchParam.PageSize,
                    PageIndex = searchParam.PageIndex,
                    Data = await _context
                                .Products
                                .Find(filter)
                                .Sort(Builders<Product>.Sort.Ascending("Name"))
                                .Skip(searchParam.PageSize * (searchParam.PageIndex - 1))
                                .Limit(searchParam.PageSize)
                                .ToListAsync(),
                    Count = await _context.Products.CountDocumentsAsync(p => true)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inside GetProducts(). Ex: {ex.InnerException}");
                throw ex;
            }
        }
        public async Task<IEnumerable<ProductType>> GetProductTypes()
        {
            try
            {
                return await _context.Types.Find(p => true).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inside GetTypes(). Ex: {ex.InnerException}");
                throw ex;
            }
        }
        public async Task<bool> UpdateProduct(Product product)
        {
            try
            {
                var status = await _context.Products.ReplaceOneAsync(p=> p.Id == product.Id,product).ConfigureAwait(false);
                return status.IsAcknowledged && status.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error inside UpdateProduct(). Ex: {ex.InnerException}");
                throw ex;
            }
        }
        public async Task<bool> UpdateProductType(ProductType productType)
        {
            var status = await _context.Types.ReplaceOneAsync(p => p.Id == productType.Id, productType).ConfigureAwait(false);
            return status.IsAcknowledged && status.ModifiedCount > 0;
        }
    }
}
