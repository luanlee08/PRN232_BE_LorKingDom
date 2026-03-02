using BLL.DTOs.Products;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Customer
{
    [ApiController]
    [Route("api/products")]
    public class CProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public CProductsController(IProductService service)
        {
            _service = service;
        }

        // GET: api/products?keyword=lego
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductQuery query)
        {
            var result = await _service.GetStorefrontAsync(query);
            return StatusCode(result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode(result.Status, result);
        }   
    }
}