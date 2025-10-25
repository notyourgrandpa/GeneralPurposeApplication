using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Services
{
    public interface IProductService
    {
        Task<ApiResult<ProductDTO>> GetProductsAsync(
            int pageIndex,
            int pageSize,
            string? sortColumn,
            string? sortOrder,
            string? filterColumn,
            string? filterQuery);
        Task<Product?> GetProductAsync(int productId);
        Task<ProductDTO> CreateProductAsync(ProductCreateDTO productCreateDTO);
        Task<bool> UpdateProductAsync(int productId, ProductUpdateDTO productUpdateDTO);
        Task<bool> DeleteProductAsync(int productId);
        Task UpdateStockAsync(InventoryLog inventoryLog);
    }
}
