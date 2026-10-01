using Asp.Versioning;
using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.API.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    //[Authorize(Policy = "CanReadWrite")]
    public class CatalogController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CatalogController> _logger;
        public CatalogController(IMediator mediator, ILogger<CatalogController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetAllProducts")]
        [ProducesResponseType(typeof(IList<ProductResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<ProductResponse>>> GetAllProducts([FromQuery] CatalogParams searchParam)
        {
            try
            {
                var query = new GetAllProductsQuery(searchParam);
                var result = await _mediator.Send(query);
                _logger.LogInformation("All products retrieved");
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"An Exception has occured: {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("[action]/{id}", Name = "GetProductById")]
        [ProducesResponseType(typeof(ProductResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductResponse>> GetProductById(string id)
        {
            try
            {
                var query = new GetProductByIdQuery(id);
                var result = await _mediator.Send(query);
                _logger.LogInformation("product by id retrieved");
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"An Exception has occured: {e.Message}");
                throw;
            }
        }
        [HttpGet]
        [Route("GetAllBrands")]
        [ProducesResponseType(typeof(IList<BrandResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<BrandResponse>>> GetAllBrands()
        {
            try
            {
                var query = new GetAllBrandsQuery();
                var result = await _mediator.Send(query);
                _logger.LogInformation("All brands retrieved");
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"An Exception has occured: {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("GetAllTypes")]
        [ProducesResponseType(typeof(IList<TypeResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<TypeResponse>>> GetAllTypes()
        {
            try
            {
                var query = new GetAllTypesQuery();
                var result = await _mediator.Send(query);
                _logger.LogInformation("All types retrieved");
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"An Exception has occured: {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("[action]/{brand}", Name = "GetProductsByBrandName")]
        [ProducesResponseType(typeof(IList<ProductResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<ProductResponse>>> GetProductsByBrandName(string brand)
        {
            try
            {
                var query = new GetProductByBrandQuery(brand);
                var result = await _mediator.Send(query);
                _logger.LogInformation("get products by brand name retrieved");
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"An Exception has occured: {e.Message}");
                throw;
            }
        }

        [HttpGet]
        [Route("[action]/{productName}", Name = "GetProductByProductName")]
        [ProducesResponseType(typeof(IList<ProductResponse>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IList<ProductResponse>>> GetProductByProductName(string productName)
        {

            try
            {
                var query = new GetProductByNameQuery(productName);
                var result = await _mediator.Send(query);
                _logger.LogInformation("get products by product name retrieved");
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"An Exception has occured: {e.Message}");
                throw;
            }
        }

        [HttpPost]
        [Route("CreateProduct")]
        [ProducesResponseType(typeof(ProductResponse), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductCommand productCommand)
        {
            try
            {
                var result = await _mediator.Send(productCommand);
                _logger.LogInformation("product created");
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"An Exception has occured: {e.Message}");
                throw;
            }
        }

        [HttpPut]
        [Route("UpdateProduct")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand productCommand)
        {

            try
            {
                var result = await _mediator.Send(productCommand);
                _logger.LogInformation("product updated");
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError($"An Exception has occured: {e.Message}");
                throw;
            }
        }

        [HttpDelete]
        [Route("{id}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(bool), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> DeleteProduct(string id)
        {

            try
            {
                var query = new DeleteProductByIdCommand(id);
                bool result = await _mediator.Send(query);
                if (result)
                {
                    _logger.LogInformation("product deletion succeeded");
                    return Ok(result);
                }
                else
                {
                    _logger.LogInformation("product deletion failed");
                    return BadRequest(result);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"An Exception has occured: {e.Message}");
                throw;
            }
        }
    }
}
