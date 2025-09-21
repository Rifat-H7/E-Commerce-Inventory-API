using E_Commerce_Inventory.Application.DTOs;
using E_Commerce_Inventory.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Inventory.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Create a new product
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.CreateAsync(createDto);

            if (result.Success)
                return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Get all products with filters and pagination
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProductQueryDto queryDto)
        {
            var result = await _productService.GetAllAsync(queryDto);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Get product by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productService.GetByIdAsync(id);

            if (result.Success)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Update product
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _productService.UpdateAsync(id, updateDto);

            if (result.Success)
                return Ok(result);

            if (result.Message == "Product not found" || result.Message == "Category not found")
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Delete product
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);

            if (result.Success)
                return Ok(result);

            if (result.Message == "Product not found")
                return NotFound(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Search products by name or description
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest(new ApiResponseDto<IEnumerable<ProductDto>>
                {
                    Success = false,
                    Message = "Search keyword cannot be empty"
                });

            var result = await _productService.SearchAsync(keyword);

            if (result.Success)
                return Ok(result);

            return NotFound(result);
        }
    }
}
