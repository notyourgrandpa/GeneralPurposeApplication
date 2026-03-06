using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.QueryParameters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Categories.Queries
{
    public class GetCategoriesHandler
    {
        private readonly IApplicationDbContext _context;

        public GetCategoriesHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResult<CategoryDTO>> Handle(QueryParameter parameters)
        {
            return await ApiResult<CategoryDTO>.CreateAsync(
                 _context.Categories
                .AsNoTracking()
                .Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    TotalProducts = c.Products.Count(),
                }), parameters);

        }
    }
}
