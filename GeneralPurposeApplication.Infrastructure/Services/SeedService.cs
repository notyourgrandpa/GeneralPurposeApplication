using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Path = System.IO.Path;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using GeneralPurposeApplication.Application.Services;
using GeneralPurposeApplication.Domain.Abstractions;
using GeneralPurposeApplication.Infrastructure.Identity;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Domain.Categories;
using GeneralPurposeApplication.Domain.Products;

namespace GeneralPurposeApplication.Infrastructure.Services
{
    public class SeedService : ISeedService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _configuration;

        public SeedService(IUnitOfWork unitOfWork, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IHostingEnvironment env, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
            _env = env;
            _configuration = configuration;
        }

        public Task CreateDefaultUser()
        {
            throw new NotImplementedException();
        }

        public async Task<SeedResultDTO> Import()
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
            var categoriesByName = _unitOfWork.Repository<Category>().GetQueryable()
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
                await _unitOfWork.Repository<Category>().AddAsync(category);
                categoriesByName.Add(categoryName, category);
                numberOfCategoriesAdded++;
            }

            if (numberOfCategoriesAdded > 0)
                await _unitOfWork.SaveChangesAsync();

            // Create a lookup dictionary containing all the cities already existing into the Database (it will be empty on first run). 
            var products = _unitOfWork.Repository<Product>()
                .GetQueryable()
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
                                   nRow, 1, nRow, worksheet.Dimension.End.Column];
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
                await _unitOfWork.Repository<Product>().AddAsync(product);
                numberOfProductsAdded++;
            }

            if (numberOfProductsAdded > 0)
                await _unitOfWork.SaveChangesAsync();

            return new SeedResultDTO { Categories = numberOfCategoriesAdded, Products = numberOfProductsAdded };
        }
    }
}
