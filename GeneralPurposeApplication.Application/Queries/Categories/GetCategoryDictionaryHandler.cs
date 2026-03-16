using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Queries.Categories
{
    public class GetCategoryDictionaryHandler
    {
        private readonly IApplicationDbContext _context;

        public GetCategoryDictionaryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, CategoryDTO>> Handle(GetCategoryDictionaryQuery query)
        {
            return await _context.Categories
                .AsNoTracking()
                .ToDictionaryAsync(
                    c => c.Name, 
                    c => new CategoryDTO 
                    { 
                        Id = c.Id,
                        Name = c.Name 
                    }, 
                    StringComparer.OrdinalIgnoreCase);
        }
    }
}
