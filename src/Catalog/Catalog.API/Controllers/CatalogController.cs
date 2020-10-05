using Catalog.API.Entities;
using Catalog.API.Entities.Repositories.Interfaces;
using DnsClient.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    public class CatalogController:ControllerBase
    {
        private readonly ICatalogRepository _repo;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(ICatalogRepository productRepository, ILogger<CatalogController> logger)
        {
            _repo = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>),(int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _repo.GetProducts();
            return Ok(products);
        }

        [HttpGet("{id:length(24)}",Name ="GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> GetProduct(string id)
        {
            var product = await _repo.GetProduct(id);
            if (product == null)
            {
                _logger.LogError($"Product with id: {id}, not found");
                return NotFound();
            }
            return Ok(product);
        }
        [Route("[action]/{categoryName}")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetCatalogByCategoryName(string categoryName)
        {
            var products = await _repo.GetCatalogsByCategoryName(categoryName);
            if (products == null)
            {
                _logger.LogError($"Products with category name: {categoryName}, not found");
                return NotFound();
            }
            return Ok(products);
        }

        [Route("[action]/{name}")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<Product>>> GetCatalogByName(string name)
        {
            var products = await _repo.GetCatalogsByName(name);
            if (products == null)
            {
                _logger.LogError($"Products with name: {name}, not found");
                return NotFound();
            }
            return Ok(products);
        }

        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] Product product)
        {
            await _repo.Create(product);
            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        [HttpPut]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> UpdateProduct([FromBody] Product product)
        {
            return Ok(await _repo.Update(product));
        }

        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> DeleteProduct(string id)
        {
            return Ok(await _repo.Delete(id));
        }

    }
}
