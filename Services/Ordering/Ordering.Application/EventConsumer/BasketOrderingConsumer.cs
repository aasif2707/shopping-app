using AutoMapper;
using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Application.Commands;
using Shared.Messaging.RabbitMQ.Events;
using MassTransit.Mediator;

namespace Ordering.Application.EventConsumer
{
    public class BasketOrderingConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger<BasketOrderingConsumer> _logger;
        public BasketOrderingConsumer(IMediator mediator, IMapper mapper, ILogger<BasketOrderingConsumer> logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            _logger.LogInformation($"Basket checkout event started!!!");
            var command = _mapper.Map<CreateOrderCommand>(context.Message);
            await _mediator.Send(command);
            _logger.LogInformation($"Basket checkout event completed!!!");
        }
    }
}
