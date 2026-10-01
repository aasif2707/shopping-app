using Amazon.Runtime.Internal.Util;
using AutoMapper;
using Catalog.Application.Commands;
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
    public class DeleteProductByIdCommandhandler : IRequestHandler<DeleteProductByIdCommand, bool>
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<DeleteProductByIdCommandhandler> _logger;
        private readonly IMapper _mapper;
        public DeleteProductByIdCommandhandler(IProductRepository repository, ILogger<DeleteProductByIdCommandhandler> logger, IMapper mapper)
        {
            _repository = repository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<bool> Handle(DeleteProductByIdCommand request, CancellationToken cancellationToken)
        {
            return await _repository.DeleteProduct(request.Id);
        }
    }
}
