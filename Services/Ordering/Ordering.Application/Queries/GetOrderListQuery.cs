using MediatR;
using Ordering.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Queries
{
    public class GetOrderListQuery : IRequest<List<OrderResponse>>
    {
        public bool _AsNoTracking { get; set; }
        public GetOrderListQuery(bool AsNoTracking)
        {
            _AsNoTracking = AsNoTracking;
        }
    }
}
