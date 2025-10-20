using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.Models;
using GeneralPurposeApplication.Server.Data.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using GeneralPurposeApplication.Server.Services;

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
        public async Task<ApiResult<ProductDTO>> GetProducts(
            int pageIndex = 0, 
            int pageSize = 10, 
            string? sortColumn = null,
            string? sortOrder = null, 
            string? filterColumn = null, 
            string? filterQuery = null)
        {
            return await _productService.GetProductsAsync(pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        // PUT: api/Products/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct(int id, ProductUpdateDTO product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var existingProduct = await _context.Products.FindAsync(id);

            if(existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = product.Name;
            existingProduct.CostPrice = product.CostPrice;
            existingProduct.SellingPrice = product.SellingPrice;
            existingProduct.IsActive = product.IsActive;
            existingProduct.LastUpdated = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Products
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPost]
        public async Task<ActionResult<Product>> PostProduct(ProductCreateDTO product)
        {
            var newProduct = new Product 
            { 
                Name = product.Name, 
                CostPrice = product.CostPrice ,
                SellingPrice = product.SellingPrice,
                IsActive = product.IsActive,
                LastUpdated = DateTime.UtcNow,
                DateAdded = DateTime.UtcNow,
                CategoryId = product.CategoryId,
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = newProduct.Id }, newProduct);
        }

        // DELETE: api/Products/5
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
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
