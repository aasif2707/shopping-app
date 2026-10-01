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
    public class GetAllTypesQueryHandler : MediatR.IRequestHandler<GetAllTypesQuery, IList<TypeResponse>>
    {
        private readonly ITypeRepository _repository;
        private readonly ILogger<GetAllTypesQueryHandler> _logger;
        private readonly IMapper _mapper;
        public GetAllTypesQueryHandler(ITypeRepository repository, ILogger<GetAllTypesQueryHandler> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<IList<TypeResponse>> Handle(GetAllTypesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var types = await _repository.GetProductTypes();
                //return _mapper.Map<IList<TypeResponse>>(types);
                return CustomMapper.Mapper.Map<IList<ProductType>,IList<TypeResponse>>(types.ToList());
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Exception inside GetProductByIdQueryHandler. {ex.Message}");
                throw ex;
            }
        }
    }
}
