using Amazon.Runtime.Internal.Util;
using AutoMapper;
using Catalog.Application.Mapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
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
    public class GetProductByIdQueryHandler : MediatR.IRequestHandler<GetProductByIdQuery, ProductResponse>
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<GetProductByIdQueryHandler> _logger;
        private readonly IMapper _mapper;
        public GetProductByIdQueryHandler(IProductRepository repository, ILogger<GetProductByIdQueryHandler> logger, IMapper mapper) 
        {
            _repository  = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<ProductResponse> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _repository.GetProductById(request.Id);
                //return _mapper.Map<ProductResponse>(product);
                return CustomMapper.Mapper.Map<ProductResponse>(product);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Exception inside GetProductByIdQueryHandler. {ex.Message}");
                return null;
            }
        }
    }
}
