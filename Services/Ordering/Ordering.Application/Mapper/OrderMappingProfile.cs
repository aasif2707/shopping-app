using AutoMapper;
using Ordering.Application.Commands;
using Ordering.Application.Responses;
using Ordering.Core.Entites;

namespace Ordering.Application.Mapper
{
    public class OrderMappingProfile :Profile
    {
        public OrderMappingProfile() 
        {
            CreateMap<Order, OrderResponse>().ReverseMap();
            CreateMap<Order, CreateOrderCommand>().ReverseMap();
            CreateMap<CreateOrderCommand, Order>();
            CreateMap<Order, UpdateOrderCommand>().ReverseMap();
            //CreateMap<CreateOrderCommand, BasketCheckoutEvent>().ReverseMap();
        }
    }
}
