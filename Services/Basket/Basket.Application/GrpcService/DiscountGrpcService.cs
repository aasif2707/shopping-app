using Discount.Application.Protos.Grpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Discount.Application.Protos.Grpc.DiscountProtosService;

namespace Basket.Application.GrpcService
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtosService.DiscountProtosServiceClient _discountProtosServiceClient;
        public DiscountGrpcService(DiscountProtosService.DiscountProtosServiceClient discountProtosServiceClient)
        {
            _discountProtosServiceClient = discountProtosServiceClient;
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            var discountRequest = new GetDiscountRequest { ProductName = productName };
            return await _discountProtosServiceClient.GetDiscountAsync(discountRequest);
        }
    }
}
