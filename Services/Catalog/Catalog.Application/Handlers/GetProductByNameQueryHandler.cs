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
    public class GetProductByNameQueryHandler : MediatR.IRequestHandler<GetProductByNameQuery, IList<ProductResponse>>
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<GetProductByNameQueryHandler> _logger;
        private readonly IMapper _mapper;

        public GetProductByNameQueryHandler(IProductRepository repository, ILogger<GetProductByNameQueryHandler> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<IList<ProductResponse>> Handle(GetProductByNameQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var products = await _repository.GetProductByName(request.Name);
                //return _mapper.Map<IList<ProductResponse>>(products);
                return CustomMapper.Mapper.Map<IList<Product>,IList<ProductResponse>>(products.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception inside GetProductByNameQueryHandler. {ex.Message}");
                return null;
            }
        }
    }
}
