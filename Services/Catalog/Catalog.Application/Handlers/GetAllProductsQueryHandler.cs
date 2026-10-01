using Amazon.Runtime.Internal;
using Amazon.Runtime.Internal.Util;
using AutoMapper;
using Catalog.Application.Mapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core;
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
    public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, Pagination<ProductResponse>>
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<GetAllProductsQueryHandler> _logger;
        private readonly IMapper _mapper;
        public GetAllProductsQueryHandler(IProductRepository repository, ILogger<GetAllProductsQueryHandler> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<Pagination<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var productList = await _repository.GetProducts(request.searchParam);
                //var productResponseList = _mapper.Map<Pagination<ProductResponse>>(productList);
                var productResponseList = CustomMapper.Mapper.Map<Pagination<ProductResponse>>(productList);
                _logger.LogDebug("Received Product List.Total Count: {productList}", productResponseList.Count);
                return productResponseList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception inside GetAllProductsQueryHandler. {ex.Message}");
                throw ex;
            }
        }
    }
}
