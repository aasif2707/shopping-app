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
    public class GetAllBrandsQueryHandler : MediatR.IRequestHandler<GetAllBrandsQuery, IList<BrandResponse>>
    {
        private readonly IBrandRepository _repository;
        private readonly ILogger<GetAllBrandsQueryHandler> _logger;
        private readonly IMapper _mapper;
        public GetAllBrandsQueryHandler(IBrandRepository repository, ILogger<GetAllBrandsQueryHandler> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }
        public async Task<IList<BrandResponse>> Handle(GetAllBrandsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var types = await _repository.GetBrands();
                //return _mapper.Map<IList<BrandResponse>>(types);
                return CustomMapper.Mapper.Map<IList<ProductBrand>,IList<BrandResponse>>(types.ToList());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception inside GetAllBrandsQueryHandler. {ex.Message}");
                throw ex;
            }
        }
    }
}
