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

        public async Task<ApiResult<SalesTransactionsDTO>> GetSalesTransactionsAsync(int pageIndex, int pageSize, string? sortColumn, string? sortOrder, string? filterColumn, string? filterQuery)
        {
            return await ApiResult<SalesTransactionsDTO>.CreateAsync(
                _unitOfWork.Repository<SalesTransaction>().GetQueryable()
                    .Select(x => new SalesTransactionsDTO
                    {
                        Id = x.Id,
                        TotalAmount = x.TotalAmount,
                        PaymentMethod = x.PaymentMethod,
                        ProcessedByUserId = x.ProcessedByUserId,
                        //ProcessedByUserName = x.ProcessedByUser.UserName!,
                        Date = x.Date,
                    }),
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
        }

        public async Task<SalesTransaction?> GetSalesTransactionAsync(int id)
        {
            return await _unitOfWork.Repository<SalesTransaction>().GetByIdAsync(id);
        }

        public async Task<SalesTransactionsDTO> CreateSalesTransactionAsync(SalesTransactionCreateDTO salesTransactionDTO, string userId)
        {
            var productIds = salesTransactionDTO.Items.Select(i => i.ProductId).ToList();

            var validIds = await _unitOfWork.Repository<Product>().GetQueryable()
                .Where(p => productIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();

            var invalidIds = productIds.Except(validIds).ToList();
            if (invalidIds.Any())
                throw new InvalidOperationException($"Invalid product IDs: {string.Join(", ", invalidIds)}");

            var salesTransaction = new SalesTransaction
            {
                CustomerId = salesTransactionDTO.CustomerId,
                PaymentMethod = salesTransactionDTO.PaymentMethod,
                ProcessedByUserId = userId,
                Date = DateTime.UtcNow,
                TotalAmount = salesTransactionDTO.Items.Sum(i => i.Quantity * i.UnitPrice)
            };

            foreach (var row in salesTransactionDTO.Items)
            {
                SalesTransactionItem salesTransactionItem = new()
                {
                    ProductId = row.ProductId,
                    Quantity = row.Quantity,
                    UnitPrice = row.UnitPrice,
                    Subtotal = row.Quantity * row.UnitPrice
                };
                salesTransaction.SalesTransactionItems.Add(salesTransactionItem);

                InventoryLogCreateDto inventoryLog = new()
                {
                    ProductId = row.ProductId,
                    Quantity = row.Quantity,
                    ChangeType = InventoryChangeType.StockOut
                };
                await _inventoryLogService.CreateInventoryLogAsync(inventoryLog);
            }

            await _unitOfWork.Repository<SalesTransaction>().AddAsync(salesTransaction);

            await _unitOfWork.SaveChangesAsync();

            return new SalesTransactionsDTO
            {
                Id = salesTransaction.Id,
                TotalAmount = salesTransaction.TotalAmount,
                PaymentMethod = salesTransaction.PaymentMethod,
                ProcessedByUserId = salesTransaction.ProcessedByUserId,
                //ProcessedByUserName = salesTransaction.ProcessedByUser.UserName!,
                Date = salesTransaction.Date,
            };
        }

        public async Task DeleteSalesTransactionAsync(int id)
        {
            var salesTransaction = await GetSalesTransactionAsync(id);
            if (salesTransaction == null)
                throw new KeyNotFoundException();

            _unitOfWork.Repository<SalesTransaction>().Delete(salesTransaction);
            await _unitOfWork.SaveChangesAsync();
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
                    product.LastUpdated = DateTime.UtcNow;
                }
            }

            await _unitOfWork.SaveChangesAsync();
        }
    }
}
