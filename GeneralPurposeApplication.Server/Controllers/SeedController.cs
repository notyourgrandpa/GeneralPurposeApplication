using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security;

namespace GeneralPurposeApplication.Server.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public SeedController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public async Task<ActionResult> Import()
        {
            // Prevents non-development environments from running this method
            if (!_env.IsDevelopment())
                throw new SecurityException("Not allowed");
            var path = Path.Combine(_env.ContentRootPath, "Data/Source/pinoy_products.xlsx");
            using var stream = System.IO.File.OpenRead(path);
            using var excelPackage = new ExcelPackage(stream);
            var worksheet = excelPackage.Workbook.Worksheets[0];
            var nEndRow = worksheet.Dimension.End.Row;
            var numberOfCategoriesAdded = 0;
            var numberOfProductsAdded = 0;

            // Create a lookup dictionary containing all the categories already existing into the Database (it will be empty on first run).
            var categoriesByName = _context.Categories
                .AsNoTracking()
                .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);

            // Iterates through all rows, skipping the first one 
            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[
                                   nRow,
               1, nRow, worksheet.Dimension.End.Column];
                var categoryName = row[nRow, 2].GetValue<string>();

                if (categoriesByName.ContainsKey(categoryName))
                    continue;

                var category = new Category
                {
                    Name = categoryName
                };
                await _context.Categories.AddAsync(category);
                categoriesByName.Add(categoryName, category);
                numberOfCategoriesAdded++;
            }

            if (numberOfCategoriesAdded > 0)
                await _context.SaveChangesAsync();
            // Create a lookup dictionary containing all the cities already existing into the Database (it will be empty on first run). 
            var products = _context.Products
                .AsNoTracking()
                .ToDictionary(x => (
                Name: x.Name,
                CostPrice: x.CostPrice,
                SellingPrice: x.SellingPrice,
                Category: x.CategoryId,
                IsActive: x.IsActive,
                DateAdded: x.DateAdded,
                LastUpdated: x.LastUpdated
                ));

            for (int nRow = 2; nRow <= nEndRow; nRow++)
            {
                var row = worksheet.Cells[
                                   nRow,
               1, nRow, worksheet.Dimension.End.Column];
                var name = row[nRow, 1].GetValue<string>();
                var categoryName = row[nRow, 2].GetValue<string>();
                var costPrice = row[nRow, 3].GetValue<decimal>();
                var sellingPrice = row[nRow, 4].GetValue<decimal>();
                var isActive = row[nRow, 5].GetValue<bool>();
                var dateAdded = row[nRow, 6].GetValue<DateTime>();
                var lastUpdated = row[nRow, 7].GetValue<DateTime>();

                // Retrieve category Id by categoryName
                var categoryId = categoriesByName[categoryName].Id;

                if (products.ContainsKey((
                                   Name: name,
                                   CostPrice: costPrice,
                                   SellingPrice: sellingPrice,
                                   Category: categoryId,
                                   IsActive: isActive,
                                   DateAdded: dateAdded,
                                   LastUpdated: lastUpdated)))
                    continue;

                // create the product entity and fill it with xlsx data 
                var product = new Product
                {
                    Name = name,
                    CostPrice = costPrice,
                    SellingPrice = sellingPrice,
                    CategoryId = categoryId,
                    IsActive = isActive,
                    DateAdded = dateAdded,
                    LastUpdated = lastUpdated
                };
                _context.Products.Add(product);
                numberOfProductsAdded++;
            }

            if (numberOfProductsAdded > 0)
                await _context.SaveChangesAsync();


            return new JsonResult(new
            {
                Products = numberOfProductsAdded,
                Categories = numberOfCategoriesAdded
            });
        }
    }
}