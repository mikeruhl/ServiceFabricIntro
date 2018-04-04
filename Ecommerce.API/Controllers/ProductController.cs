using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using AutoMapper;
using Ecommerce.API.Model;
using Ecommerce.ProductCatalog.Model;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace Ecommerce.API.Controllers
{
    public class ProductController : ApiController
    {
        private IProductCatalogService _catalogService;

        public ProductController()
        {

            _catalogService = ServiceProxy.Create<IProductCatalogService>(new Uri("fabric:/Ecommerce/ECommerce.ProductCatalog"),
                new ServicePartitionKey(0));
        }

        [HttpGet]
        [Route("product/{productId}")]
        public async Task<IHttpActionResult> GetProduct(string productId)
        {
            var guid = Guid.Parse(productId);
            var product = await _catalogService.GetProduct(guid);

            if (product != null)
                return Ok(Mapper.Map<Product, ProductDTO>(product));
            else
                return NotFound();



        }

        [HttpGet]
        public async Task<IEnumerable<ProductDTO>> Get()
        {
            return Mapper.Map<IEnumerable<Product>, IEnumerable<ProductDTO>>(await _catalogService.GetAllProducts());
            //return new[] {new ProductDTO{Id = Guid.NewGuid(), Description = "fake"}};
        }

        [HttpPost]
        public async Task Post([FromBody] Product product)
        {
            await _catalogService.AddProduct(product);
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete([FromBody] Guid productId)
        {
            var deleted =  await _catalogService.RemoveProduct(productId);

            if (deleted)
                return Ok();
            else
                return NotFound();
        }
    }
}
