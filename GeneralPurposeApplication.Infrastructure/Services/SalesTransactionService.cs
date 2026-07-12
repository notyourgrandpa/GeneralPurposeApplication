using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.Services;
using GeneralPurposeApplication.Domain.Abstractions;
using GeneralPurposeApplication.Domain.Inventory;
using GeneralPurposeApplication.Domain.Products;
using GeneralPurposeApplication.Domain.Sales;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Infrastructure.Services
{
    public class SalesTransactionService : ISalesTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryLogService _inventoryLogService;
        public SalesTransactionService(IUnitOfWork unitOfWork, IInventoryLogService inventoryLogService)
        {
            _unitOfWork = unitOfWork;
            _inventoryLogService = inventoryLogService;
        }
        public async Task VoidSalesTransactionAsync(int id, string userId)
        {
            var transaction = await _unitOfWork.Repository<SalesTransaction>().GetQueryable().Include(t => t.SalesTransactionItems)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                throw new KeyNotFoundException("Selected transaction not found.");
            }    

            if (transaction.IsVoided)
            {
                throw new InvalidOperationException("Transaction is already voided.");
            }
                

            // Mark as voided
            transaction.IsVoided = true;
            transaction.VoidedAt = DateTime.UtcNow;
            transaction.VoidedByUserId = userId;

            // Reverse inventory changes
            foreach (var item in transaction.SalesTransactionItems)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity; // restore stock
                    product.SetUpdated(DateTime.UtcNow);
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
