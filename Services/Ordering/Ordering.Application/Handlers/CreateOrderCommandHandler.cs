using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Commands;
using Ordering.Core.Entites;
using Ordering.Core.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ordering.Application.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateOrderCommandHandler> _logger;
        public CreateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger<CreateOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<int> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            //var orderEntity = _mapper.Map<Order>(request);
            var orderEntity = new Order
            {
                AddressLine = request.AddressLine,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailAddress = request.EmailAddress,
                CreatedDate = DateTime.UtcNow,
                CreatedBy = "Asif",
                LastModifiedBy = "Asif",
                LastModifiedDate = DateTime.UtcNow,
                State = request.State,
                CardName = request.CardName,
                CardNumber = request.CardNumber,
                Country = request.Country,
                Cvv = request.CVV,
                PaymentMethod = request.PaymentMethod,
                Expiration = request.Expiration,
                UserName = request.UserName,
                TotalPrice = request.TotalPrice,
                ZipCode = request.ZipCode
            };
            var generatedOrder = await _orderRepository.AddAsync(orderEntity);
            _logger.LogInformation(($"Order {generatedOrder} successfully created."));
            return generatedOrder.Id;
        }
    }
}
