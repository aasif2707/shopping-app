using Catalog.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Core.Repositories
{
    public interface ITypeRepository
    {
        Task<IEnumerable<ProductType>> GetProductTypes();
        Task<ProductType> CreateProductType(ProductType productType);
        Task<bool> UpdateProductType(ProductType productType);
        Task<bool> DeleteProductType(string id);
    }
}
