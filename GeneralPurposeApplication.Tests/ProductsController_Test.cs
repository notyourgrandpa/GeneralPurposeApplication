using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralPurposeApplication.Server.Controllers;
using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GeneralPurposeApplication.Tests
{
    public class ProductsController_Test
    {
        /// <summary>
        /// Test the GetProduct() method
        /// </summary>
        [Fact]
        public async Task GetProduct()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "GeneralPurposeApplication")
                .Options;

            using var context = new ApplicationDbContext(options);
            context.Add(new Product()
            {
                Id = 1,
                CategoryId = 1,
                CostPrice = 1,
                SellingPrice = 1,
                IsActive = true,
                Name = "TestProduct1"
            });
            context.SaveChanges();
            var controller = new ProductsController(context);
            Product? product_existing = null;
            Product? product_notExisting = null;


            // Act
            product_existing = (await controller.GetProduct(1)).Value;
            product_notExisting = (await controller.GetProduct(2)).Value;
            // Assert
            Assert.NotNull(product_existing);
            Assert.Null(product_notExisting);
        }
    }
}
