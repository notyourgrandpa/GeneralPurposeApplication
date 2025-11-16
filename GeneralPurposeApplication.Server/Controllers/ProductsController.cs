using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Data.Models;
using GeneralPurposeApplication.Server.Services;
using Humanizer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        // GET: api/Products
        // GET: api/Cities/?pageIndex=0&pageSize=10
        // GET: api/Cities/?pageIndex=0&pageSize=10&sortColumn=name&sortOrder=asc
        [HttpGet]
        public async Task<ActionResult<ApiResult<ProductDTO>>> GetProductsAsync(
            int pageIndex = 0, 
            int pageSize = 10, 
            string? sortColumn = null,
            string? sortOrder = null, 
            string? filterColumn = null, 
            string? filterQuery = null)
        {
            var result = await _productService.GetProductsAsync(pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
            return Ok(result);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductAsync(int id)
        {
            var product = await _productService.GetProductAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductAsync(int id, ProductUpdateDTO product)
        {
            if (id != product.Id)
                return BadRequest(new { message = "ID mismatch between route and body." });

            var updated = await _productService.UpdateProductAsync(id, product);
            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductCreateDTO product)
        {
            var productDTO = await _productService.CreateProductAsync(product);

            return CreatedAtAction("GetProduct", new { id = productDTO.Id }, productDTO);
        }

        // DELETE: api/Products/5
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAsync(int id)
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }

        private async Task<bool> ProductExists(int id)
        {
            return await _productService.ProductExistsAsync(id);
        }

        [HttpPost]
        [Route("IsDupeProduct")]
        public async Task<bool> IsDupeProduct(Product product)
        {
            return await _productService.IsDupeProduct(product);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Search(string term)
        {
            return Ok(await _productService.SearchProduct(term));
        }
    }
}
