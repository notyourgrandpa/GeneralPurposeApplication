using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GeneralPurposeApplication.Server.Data.GraphQL
{
    public class Query
    {
        /// <summary>
        /// Gets all Products.
        /// </summary>
        [Serial]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Product> GetProducts([Service] ApplicationDbContext context)
            => context.Products;

        /// <summary>
        /// Gets all Products (with ApiResult and DTO support).
        /// </summary>
        [Serial]
        public async Task<ApiResult<ProductDTO>> GetProductsApiResult(
           [Service] ApplicationDbContext context,
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = null,
        string? sortOrder = null,
        string? filterColumn = null,
        string? filterQuery = null)
        {
            return await ApiResult<ProductDTO>.CreateAsync(
                       context.Products.AsNoTracking()
                           .Select(c => new ProductDTO()
                           {
                               Id = c.Id,
                               Name = c.Name,
                               SellingPrice = c.SellingPrice,
                               CostPrice = c.CostPrice,
                               IsActive = c.IsActive,
                               CategoryId = c.Category!.Id,
                               CategoryName = c.Category!.Name,
                               LastUpdated = c.LastUpdated,
                               DateAdded = c.DateAdded
                           }),
                       pageIndex,
                       pageSize,
                       sortColumn,
                       sortOrder,
                       filterColumn,
                       filterQuery);
        }

        /// <summary>
        /// Gets all Categories.
        /// </summary>
        [Serial]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Category> GetCategories([Service] ApplicationDbContext context) => context.Categories;
    }
}
