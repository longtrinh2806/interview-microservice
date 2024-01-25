using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductServices.Dtos;
using ProductServices.Services;

namespace ProductServices.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public IActionResult GetAllProducts()
        {
            var products = _productService.Get();

            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetProductById(Guid id)
        {
            var products = _productService.GetById(id);

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateNewProduct(CreateProductDto request)
        {
            var products = await _productService.Post(request);
            return Ok(products);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, UpdateProductDto request)
        {
            var result = await _productService.Put(id, request);
            
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await _productService.Delete(id);
            return Ok(result);
        }

    }
}
