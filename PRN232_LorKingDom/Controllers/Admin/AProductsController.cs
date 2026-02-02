using BLL.DTOs.Products;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_LorKingDom.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/products")]
    public class AProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public AProductsController(IProductService service)
        {
            _service = service;
        }

        /* ========================== GET ADMIN PAGED ========================== */

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductQuery query)
        {
            var result = await _service.GetAdminAsync(query);
            return StatusCode(result.Status, result);
        }

        /* ========================== GET BY ID ========================== */

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return StatusCode(result.Status, result);
        }

        /* ========================== CREATE ========================== */

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateProductRequest request)
        {
            var result = await _service.CreateAsync(request);
            return StatusCode(result.Status, result);
        }

        /* ========================== UPDATE ========================== */

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
         int id,
         [FromForm] UpdateProductRequest request)
        {
            var result = await _service.UpdateAsync(id, request);
            return StatusCode(result.Status, result);
        }
    }
}

