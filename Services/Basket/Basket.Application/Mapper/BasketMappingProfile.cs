using AutoMapper;
using Basket.Application.Responses;
using Basket.Core.Entities;
using Shared.Messaging.RabbitMQ.Events;

namespace Basket.Application.Mapper
{
    public class BasketMappingProfile :Profile
    {
        public BasketMappingProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartResponse>().ReverseMap();
            CreateMap<ShoppingCartItem, ShoppingCartItemResponse>().ReverseMap();
            CreateMap<BasketCheckout, BasketCheckoutEvent>().ReverseMap();
        }
    }
}
