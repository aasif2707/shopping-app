using Amazon.Runtime.Internal;
using Catalog.Application.Responses;
using Catalog.Core;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Queries
{
    public class GetAllProductsQuery : IRequest<Pagination<ProductResponse>>
    {
        public CatalogParams searchParam { get; set; }

        public GetAllProductsQuery(CatalogParams searchParam)
        {
            this.searchParam = searchParam;
        }
    }
}
