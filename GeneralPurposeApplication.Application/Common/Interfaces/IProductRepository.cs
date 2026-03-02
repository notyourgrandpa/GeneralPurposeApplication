using GeneralPurposeApplication.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Common.Interfaces
{
    public record ProductCompositeKey(string name, decimal sellingPrice, decimal costPrice, int categoryId);
    public interface IProductRepository
    {
        public Task AddAsync(Product product);
        public Task<IEnumerable<Product>> GetAllAsync(CancellationToken ct = default);
        public Task<HashSet<ProductCompositeKey>> GetProductCompositeKeysAsync(CancellationToken ct = default);
    }
}
