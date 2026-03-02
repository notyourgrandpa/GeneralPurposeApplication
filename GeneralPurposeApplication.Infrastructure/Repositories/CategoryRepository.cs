using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IApplicationDbContext _context;
        public CategoryRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, Category>> GetDictionaryAsync()
        {
            return await _context.Categories.AsNoTracking().ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);
        }
    }
}
