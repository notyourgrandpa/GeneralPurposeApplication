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
        private readonly ApplicationDbContext _context;
        private readonly IProductService _productService;

        public ProductsController(ApplicationDbContext context, IProductService productService)
        {
            _context = context;
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
        public bool IsDupeProduct(Product product)
        {
            return _context.Products.AsNoTracking().Any(
                e => e.Name == product.Name
                && e.CategoryId == product.CategoryId
                && e.SellingPrice == product.SellingPrice
                && e.CostPrice == product.CostPrice
                && e.Id != product.Id
            );
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Ok(new List<ProductDTO>()); // empty list if no search term

            var products = await _context.Products
                .Where(c => c.Name.Contains(term))
                .OrderBy(c => c.Name)
                .Take(20) // limit results for performance
                .Select(c => new ProductDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    SellingPrice = c.SellingPrice
                })
                .ToListAsync();

            return Ok(products);
        }
    }
}
