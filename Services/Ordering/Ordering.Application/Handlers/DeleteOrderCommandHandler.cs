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
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;
        private readonly IUnitOfWork _uow;

        public DeleteOrderCommandHandler(IOrderRepository orderRepository, ILogger<DeleteOrderCommandHandler> logger, IUnitOfWork uow)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _uow = uow;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToDelete = await _orderRepository.GetByIdAsync(request.Id);
            if (orderToDelete == null)
            {
                throw new OrderException(nameof(Order), request.Id);
            }
            try
            {
                // Without UoW
                await _orderRepository.DeleteAsync(orderToDelete);

                // With UoW
                await _uow.Orders.DeleteAsync(orderToDelete);

                _uow.Save();
                _logger.LogInformation($"Order with Id {request.Id} is deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message}");
            }
            return Unit.Value;
        }
    }
}
