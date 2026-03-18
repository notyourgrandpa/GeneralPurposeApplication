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
using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.Queries.Categories;
using GeneralPurposeApplication.Application.Commands;

namespace GeneralPurposeApplication.Infrastructure.Services
{
    public class SeedService : ISeedService
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHostingEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly GetCategoryDictionaryHandler _getCategoryDictionaryHandler;
        private readonly CreateCategoryHandler _createCategoryHandler;
        private readonly IApplicationDbContext _context;

        public SeedService(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager, 
            IHostingEnvironment env, 
            IConfiguration configuration,
            GetCategoryDictionaryHandler getCategoryDictionaryHandler,
            CreateCategoryHandler createCategoryHandler,
            IApplicationDbContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _env = env;
            _configuration = configuration;
            _getCategoryDictionaryHandler = getCategoryDictionaryHandler;
            _createCategoryHandler = createCategoryHandler;
            _context = context;
        }

        public async Task CreateDefaultUser()
        {
            // setup the default role names
            string role_RegisteredUser = "RegisteredUser";
            string role_Administrator = "Administrator";

            // create the default roles (if they don't exist yet)
            if (await _roleManager.FindByNameAsync(role_RegisteredUser) == null)
                await _roleManager.CreateAsync(new IdentityRole(role_RegisteredUser));

            if (await _roleManager.FindByNameAsync(role_Administrator) == null)
                await _roleManager.CreateAsync(new IdentityRole(role_Administrator));

            // create a list to track the newly added users
            var addedUserList = new List<ApplicationUser>();

            // check if the admin user already exists
            var email_Admin = "admin@email.com";

            if (await _userManager.FindByNameAsync(email_Admin) == null)
            {
                // create a new admin ApplicationUser account
                var user_Admin = new ApplicationUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_Admin,
                    Email = email_Admin,
                };
                // insert the admin user into the DB
                await _userManager.CreateAsync(user_Admin, _configuration["DefaultPasswords:Administrator"]);

                // assign the "RegisteredUser" and "Administrator" roles
                await _userManager.AddToRoleAsync(user_Admin, role_RegisteredUser);
                await _userManager.AddToRoleAsync(user_Admin, role_Administrator);

                // confirm the e-mail and remove lockout
                user_Admin.EmailConfirmed = true;
                user_Admin.LockoutEnabled = false;

                // add the admin user to the added users list
                addedUserList.Add(user_Admin);
            }
            // check if the standard user already exists
            var email_User = "user@email.com";

            if (await _userManager.FindByNameAsync(email_User) == null)
            {
                // create a new standard ApplicationUser account
                var user_User = new ApplicationUser()
                {
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = email_User,
                    Email = email_User
                };
                // insert the standard user into the DB
                await _userManager.CreateAsync(user_User, _configuration["DefaultPasswords:RegisteredUser"]);
                // assign the "RegisteredUser" role
                await _userManager.AddToRoleAsync(user_User,
                        role_RegisteredUser);
                // confirm the e-mail and remove lockout
                user_User.EmailConfirmed = true;
                user_User.LockoutEnabled = false;
                // add the standard user to the added users list
                addedUserList.Add(user_User);
            }
            // if we added at least one user, persist the changes into the DB
            if (addedUserList.Count > 0)
                await _context.SaveChangesAsync();
        }

        public async Task<SeedResultDTO> Import()
        {
            try
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
                var categoriesByName = await _context.Categories.ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);

                var newCategories = new List<Category>();

                // Iterates through all rows, skipping the first one 
                for (int nRow = 2; nRow <= nEndRow; nRow++)
                {
                    var row = worksheet.Cells[
                                       nRow,
                   1, nRow, worksheet.Dimension.End.Column];
                    var categoryName = row[nRow, 2].GetValue<string>();

                    if (categoriesByName.ContainsKey(categoryName))
                        continue;

                    var category = await _createCategoryHandler.Handle(new CreateCategoryCommand(categoryName));
                    newCategories.Add(category);
                    categoriesByName.Add(categoryName, category);
                    numberOfCategoriesAdded++;
                }

                if (newCategories.Count > 0)
                    await _context.SaveChangesAsync();

                // Create a lookup dictionary containing all the cities already existing into the Database (it will be empty on first run). 
                var existingKeys = _context.Products.AsNoTracking().ToHashSet();

                for (int nRow = 2; nRow <= nEndRow; nRow++)
                {
                    var row = worksheet.Cells[
                                       nRow, 1, nRow, worksheet.Dimension.End.Column];
                    var name = row[nRow, 1].GetValue<string>();
                    var categoryName = row[nRow, 2].GetValue<string>();
                    var costPrice =  row[nRow, 3].GetValue<decimal>();
                    var sellingPrice = row[nRow, 4].GetValue<decimal>();
                    //var isActive = row[nRow, 5].GetValue<bool>();

                    // Retrieve category Id by categoryName
                    var categoryId = categoriesByName[categoryName].Id;

                    var key = new Product 
                    { 
                        Name = name, 
                        CategoryId = categoryId,
                        CostPrice = costPrice,
                        SellingPrice = sellingPrice,
                    };

                    if (existingKeys.Contains(key))
                        continue;

                    key.SetCreated(DateTime.UtcNow);
                    await _context.Products.AddAsync(key);
                    numberOfProductsAdded++;
                }

                if (numberOfProductsAdded > 0)
                    await _context.SaveChangesAsync();

                return new SeedResultDTO { Categories = numberOfCategoriesAdded, Products = numberOfProductsAdded };
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.Message;
                throw new Exception(message, ex);
            }
        }
    }
}
