using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.Models;
using System.Linq.Dynamic.Core;
using GeneralPurposeApplication.Server.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using GeneralPurposeApplication.Server.Services;

namespace GeneralPurposeApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICategoryService _categoryService;

        public CategoriesController(ApplicationDbContext context, ICategoryService categoryService)
        {
            _context = context;
            _categoryService = categoryService;
        }

        // GET: api/Categories
        [HttpGet]
        public async Task<ActionResult<ApiResult<CategoryDTO>>> GetCategories(
            int pageIndex = 0,
            int pageSize = 10,
            string? sortColumn = null,
            string? sortOrder = null,
            string? filterColumn = null,
            string? filterQuery = null)
        {
            return await _categoryService.GetCategoriesAsync(pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            //var category = await _context.Categories.FindAsync(id);
            var category = await _categoryService.GetCategoryAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "RegisteredUser")]
        public async Task<IActionResult> PutCategory(int id, CategoryUpdateDTO category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            try
            {
                await _categoryService.UpdateCategoryAsync(id, category);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
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

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "RegisteredUser")]
        [HttpPost]
        public async Task<ActionResult<Category>> PostCategory(CategoryCreateInputDTO category)
        {
            var newCategory = await _categoryService.CreateCategoryAsync(category);

            return CreatedAtAction("GetCategory", new { id = newCategory.Id }, newCategory);
        }

        // DELETE: api/Categories/5
        [Authorize(Roles = "Administrator")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleted = await _categoryService.DeleteCategoryAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            s
            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }

        [HttpPost]
        [Route("IsDupeField")]
        public bool IsDupeField(int categoryId, string fieldName, string fieldValue)
        {
            //// Default approach (using strongly-typed LAMBA expressions)
            //switch (fieldName)
            //{
            //    case "name":
            //        return _context.Categories.Any(
            //            c => c.Name == fieldValue && c.Id != categoryId);

            //    default:
            //        return false;
            //}

            // Alternative approach (using System.Linq.Dynamic.Core)
            return (ApiResult<Category>.IsValidProperty(fieldName, true))
                ? _context.Categories.Any(
                    string.Format("{0} == @0 && Id != @1", fieldName),
                    fieldValue,
                    categoryId)
                : false;
        }
    }
}
