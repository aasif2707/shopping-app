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
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateOrderCommandHandler> _logger;
        private readonly IUnitOfWork _uow;
        public UpdateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger<UpdateOrderCommandHandler> logger, IUnitOfWork uow)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }
        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToUpdate = await _orderRepository.GetByIdAsync(request.Id);
            if (orderToUpdate == null)
            {
                throw new OrderException(nameof(Order), request.Id);
            }

            try
            {
                _mapper.Map(request, orderToUpdate, typeof(UpdateOrderCommand), typeof(Order));
                //without UoW
                await _orderRepository.UpdateAsync(orderToUpdate);

                //With UoW
                await _uow.Orders.UpdateAsync(orderToUpdate);
                _uow.Save();
                _logger.LogInformation($"Order {orderToUpdate.Id} is successfully updated");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return Unit.Value;
        }
    }
}
