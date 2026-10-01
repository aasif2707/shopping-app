using AutoMapper;
using Catalog.Application.Mapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Handlers
{
    public class GetProductByBrandQueryHandler : MediatR.IRequestHandler<GetProductByBrandQuery, IList<ProductResponse>>
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<GetProductByBrandQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetProductByBrandQueryHandler(IProductRepository repository, ILogger<GetProductByBrandQueryHandler> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<IList<ProductResponse>> Handle(GetProductByBrandQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var products = await _repository.GetProductByBrand(request.Brandname);
                //return _mapper.Map<IList<ProductResponse>>(products);
                return CustomMapper.Mapper.Map<IList<Product>,IList<ProductResponse>>(products.ToList());
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Exception inside GetProductByBrandQueryHandler. {ex.Message}");
                return null;
            }
        }
    }
}
