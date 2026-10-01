using MediatR;
using Ordering.Application.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Queries
{
    public class GetOrderByUserNameQuery :IRequest<List<OrderResponse>>
    {
        public string UserName { get; set; }
        public GetOrderByUserNameQuery(string userName)
        {
            UserName = userName;
        }
    }
}
