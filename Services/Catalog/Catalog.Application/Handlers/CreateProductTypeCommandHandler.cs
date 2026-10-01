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
    public class CreateProductTypeCommandHandler : IRequestHandler<CreateProductTypeCommand, TypeResponse>
    {
        private readonly ITypeRepository _repository;
        private readonly ILogger<CreateProductTypeCommandHandler> _logger;
        private readonly IMapper _mapper;
        public CreateProductTypeCommandHandler(ITypeRepository repository, ILogger<CreateProductTypeCommandHandler> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<TypeResponse> Handle(CreateProductTypeCommand request, CancellationToken cancellationToken)
        {
            var result = _mapper.Map<ProductType>(request);
            if (result == null) 
            {
                throw new ApplicationException("There is an issue with mapping while creating new product-types");
            }
            var newType = await _repository.CreateProductType(result);
            var typeResponse = _mapper.Map<TypeResponse>(newType);
            return typeResponse;
        }
    }
}
