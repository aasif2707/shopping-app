using Amazon.Runtime.Internal;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Commands
{
    public class CreateProductBrandCommand : IRequest<BrandResponse>
    {
        public string BrandName { get; set; }
    }
}
