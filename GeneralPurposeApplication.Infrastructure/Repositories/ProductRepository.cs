using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Domain.Products;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IApplicationDbContext _context;
        public ProductRepository(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product) => await _context.Products.AddAsync(product);
        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct) => await _context.Products.AsNoTracking().ToListAsync(ct);

        public async Task<HashSet<ProductCompositeKey>> GetProductCompositeKeysAsync(CancellationToken ct = default)
        {
            var list = await _context.Products
                .AsNoTracking()
                .Select(p => new ProductCompositeKey(
                    p.Name,
                    p.SellingPrice,
                    p.CostPrice,
                    p.CategoryId))
                .ToListAsync(ct);

            return list.ToHashSet();
        }
    }
}
