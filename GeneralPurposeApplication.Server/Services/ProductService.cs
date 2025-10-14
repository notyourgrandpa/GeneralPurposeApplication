using GeneralPurposeApplication.Server.Data.Models;
using GeneralPurposeApplication.Server.Repositories;
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
    }
}
