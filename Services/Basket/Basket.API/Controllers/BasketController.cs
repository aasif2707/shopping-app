using Asp.Versioning;
using Basket.Application.Commands;
using Basket.Application.GrpcService;
using Basket.Application.Mapper;
using Basket.Application.Queries;
using Basket.Application.Responses;
using Basket.Core.Entities;
using MassTransit;
using MediatR;
using Shared.Messaging.RabbitMQ.Events;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BasketController> _logger;
        private readonly DiscountGrpcService _discountGrpcService;
        private readonly IPublishEndpoint _publishEndpoint;
        public BasketController(IMediator mediator, ILogger<BasketController> logger, DiscountGrpcService discountGrpcService, IPublishEndpoint publishEndpoint) 
        {
            _mediator = mediator;
            _logger = logger;
            _discountGrpcService = discountGrpcService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        [Route("[action]/{userName}", Name = "GetBasketByUserName")]
        [ProducesResponseType(typeof(ShoppingCartResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCartResponse>> GetBasket(string userName)
        {

            try
            {
                var query = new GetBasketByUserNameQuery(userName);
                var basket = await _mediator.Send(query);
                _logger.LogInformation("basket retrieved");
                return Ok(basket);
            }
            catch (Exception e)
            {
                _logger.LogError($"An Exception has occured: {e.Message}");
                throw;
            }
        }

        [HttpPost("CreateBasket")]
        [ProducesResponseType(typeof(ShoppingCartResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCartResponse>> UpdateBasket([FromBody] CreateShoppingCartCommand createShoppingCartCommand)
        {
            try
            {
                foreach (var item in createShoppingCartCommand.Items)
                {
                    _logger.LogInformation($"gRPC service call started.");
                    var coupon = await _discountGrpcService.GetDiscount(item.ProductName);
                    _logger.LogInformation($"Discount of {coupon} fetched for product {item.ProductName}");
                    item.Price -= coupon.Amount;
                    _logger.LogInformation($"gRPC service call ended. Discount applied.");
                }
                var basket = await _mediator.Send(createShoppingCartCommand);
                return Ok(basket);
            }
            catch (Exception ex) 
            {
                _logger.LogError($"Some error happened due to gRPC.");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("[action]/{userName}", Name = "DeleteBasketByUserName")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCartResponse>> DeleteBasket(string userName)
        {
            var query = new DeleteBasketByUserNameCommand(userName);
            return Ok(await _mediator.Send(query));
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            try
            {
                var query = new GetBasketByUserNameQuery(basketCheckout.UserName);
                var basket = await _mediator.Send(query);
                if (basket == null)
                {
                    return BadRequest();
                }

                _logger.LogInformation($"Pushing data to rabbitMQ");
                var eventMesg = BasketMapper.Mapper.Map<BasketCheckoutEvent>(basketCheckout);
                eventMesg.TotalPrice = basket.TotalPrice;
                await _publishEndpoint.Publish(eventMesg);
                _logger.LogInformation($"Deleting data from basket post success push to rabbitMQ");
                var deleteQuery = new DeleteBasketByUserNameCommand(basketCheckout.UserName);
                await _mediator.Send(deleteQuery);
                return Accepted();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Some error happened while pushing to rabbitMQ");
                return BadRequest(ex.Message);
            }
        }
    }
}
