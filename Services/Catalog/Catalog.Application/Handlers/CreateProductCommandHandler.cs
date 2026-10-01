using AutoMapper;
using Catalog.Application.Commands;
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
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<CreateProductCommandHandler> _logger;
        private readonly IMapper _mapper;
        public CreateProductCommandHandler(IProductRepository repository, ILogger<CreateProductCommandHandler> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productEntity = _mapper.Map<Product>(request);
            if (productEntity is null)
            {
                throw new ApplicationException("There is an issue with mapping while creating new product");
            }

            var newProduct = await _repository.CreateProduct(productEntity);
            var productResponse = _mapper.Map<ProductResponse>(newProduct);
            return productResponse;
        }
    }
}
