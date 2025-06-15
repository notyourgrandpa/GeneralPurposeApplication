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
        /// Gets all Categories.
        /// </summary>
        [Serial]
        [UsePaging]
        [UseFiltering]
        [UseSorting]
        public IQueryable<Category> GetCategories([Service] ApplicationDbContext context) => context.Categories;
    }
}
