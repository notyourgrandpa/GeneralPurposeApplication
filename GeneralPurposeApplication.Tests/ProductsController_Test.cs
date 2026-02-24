using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralPurposeApplication.Domain.Products;
using GeneralPurposeApplication.Infrastructure.Persistence;
using GeneralPurposeApplication.Infrastructure.Repositories;
using GeneralPurposeApplication.Infrastructure.Services;
using GeneralPurposeApplication.Server.Controllers;
using GeneralPurposeApplication.Server.Data;
using Microsoft.AspNetCore.Mvc;
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

            var unitOfWork = new UnitOfWork(context);
            var productService = new ProductService(unitOfWork);
            var controller = new ProductsController(productService);
            //Product? product_existing = null;
            //Product? product_notExisting = null;


            // Act & Assert existing product
            var result_existing = await controller.GetProductAsync(1);
            var okResult_existing = Assert.IsType<OkObjectResult>(result_existing.Result);
            var product_existing = Assert.IsType<Product>(okResult_existing.Value);
            Assert.NotNull(product_existing);

            // Act & Assert non-existing product
            var result_notExisting = await controller.GetProductAsync(2);
            Assert.IsType<NotFoundResult>(result_notExisting.Result);
        }
    }
}
