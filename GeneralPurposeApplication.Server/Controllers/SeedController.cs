using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace GeneralPurposeApplication.Server.Controllers
{
    //[Authorize(Roles = "Administrator")]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;

        public SeedController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, IWebHostEnvironment env, IConfiguration configuration)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
            _env = env;
            _configuration = configuration;
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

        [HttpGet]
        public async Task<ActionResult> CreateDefaultUsers()
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

            return new JsonResult(new
            {
                Count = addedUserList.Count,
                Users = addedUserList
            });
        }
    }
}
