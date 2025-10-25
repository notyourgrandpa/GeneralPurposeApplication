using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Data.Models;
using GeneralPurposeApplication.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Services
{
    public class ProductService: IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<ProductDTO>> GetProductsAsync(int pageIndex, int pageSize, string? sortColumn, string? sortOrder, string? filterColumn, string? filterQuery)
        {
            return await ApiResult<ProductDTO>.CreateAsync(
                _unitOfWork.Repository<Product>().GetQueryable()
                .AsNoTracking()
                .Select(x => new ProductDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category!.Name,
                    CostPrice = x.CostPrice,
                    SellingPrice = x.SellingPrice,
                    Stock = x.Stock,
                    IsActive = x.IsActive,
                    DateAdded = x.DateAdded,
                    LastUpdated = x.LastUpdated,
                }),
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
        }

        public async Task<Product?> GetProductAsync(int productId)
        {
            return await _unitOfWork.Repository<Product>().GetByIdAsync(productId);
        }

        public async Task<ProductDTO> CreateProductAsync(ProductCreateDTO productCreateDTO)
        {
            var product = new Product
            {
                Name = productCreateDTO.Name,
                CostPrice = productCreateDTO.CostPrice,
                SellingPrice = productCreateDTO.SellingPrice,
                IsActive = productCreateDTO.IsActive,
                LastUpdated = DateTime.UtcNow,
                DateAdded = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Product>().AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            var productDTO = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                CostPrice = product.CostPrice,
                SellingPrice = product.SellingPrice,
                IsActive = product.IsActive,
                LastUpdated = DateTime.UtcNow,
                DateAdded = DateTime.UtcNow
            };

            return productDTO;
        }

        public async Task<bool> UpdateProductAsync(int productId, ProductUpdateDTO productUpdateDTO)
        {
            var product = await GetProductAsync(productId);
            if (product == null)
                return false;

            product.Name = productUpdateDTO.Name;
            product.CostPrice = productUpdateDTO.CostPrice;
            product.SellingPrice = productUpdateDTO.SellingPrice;
            product.IsActive = productUpdateDTO.IsActive;
            product.LastUpdated = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            var product= await GetProductAsync(productId);
            if (product == null)
                return false;
            _unitOfWork.Repository<Product>().Delete(product);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task UpdateStockAsync(InventoryLog inventoryLog)
        {
            Product? product = await _unitOfWork.Repository<Product>().GetByIdAsync(inventoryLog.ProductId);
            if (product == null)
                throw new InvalidOperationException($"Product {inventoryLog.ProductId} not found.");

            if (inventoryLog.ChangeType == InventoryChangeType.StockIn)
                product.Stock += inventoryLog.Quantity;
            else if (inventoryLog.ChangeType == InventoryChangeType.StockOut)
            {
                if (product.Stock < inventoryLog.Quantity)
                    throw new InvalidOperationException("Not enough stock available.");
                product.Stock -= inventoryLog.Quantity;
            }
            else if (inventoryLog.ChangeType == InventoryChangeType.Adjustment)
                product.Stock = inventoryLog.Quantity;

            product.LastUpdated = DateTime.Now;
        }

        private async Task<bool> ProductExistsAsync(int id)
        {
            return await _unitOfWork.Repository<Product>().AnyAsync(x => x.Id == id);
        }
    }
}
