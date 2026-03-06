using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Domain.Categories;
using GeneralPurposeApplication.Domain.Products;
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

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
        }

        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken ct)
        {
            return await _context.Categories.AsNoTracking().ToListAsync();
        }

        public async Task<Dictionary<string, Category>> GetDictionaryAsync()
        {
            return await _context.Categories.AsNoTracking().ToDictionaryAsync(c => c.Name, StringComparer.OrdinalIgnoreCase);
        }
    }
}
