using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.QueryParameters;
using GeneralPurposeApplication.Domain.Inventory;
using GeneralPurposeApplication.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Services
{
    public interface IProductService
    {
        Task<PagedResult<ProductDTO>> GetProductsAsync(ProductQueryParameter parameters);
        Task<Product?> GetProductAsync(int productId);
        Task<ProductDTO> CreateProductAsync(ProductCreateDTO productCreateDTO);
        Task<bool> UpdateProductAsync(int productId, ProductUpdateDTO productUpdateDTO);
        Task<bool> DeleteProductAsync(int productId);
        Task UpdateStockAsync(InventoryLog inventoryLog);
        Task<bool> IsDupeProduct(Product product);
        Task<bool> ProductExistsAsync(int productId);
        Task<IEnumerable<ProductDTO>> SearchProduct(string term);
    }
}
