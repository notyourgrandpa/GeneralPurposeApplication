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
        Task<ApiResult<ProductDTO>> GetProduct(int id);
        Task<Product> GetProductAsync(int productId);
        Task UpdateProductAsync(int productId, ProductUpdateDTO productUpdateDTO);
        Task<bool> DeleteProductAsync(int productId);
        Task UpdateStockAsync(InventoryLog inventoryLog);
    }
}
