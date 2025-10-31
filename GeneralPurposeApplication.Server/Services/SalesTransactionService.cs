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
    public class SalesTransactionService : ISalesTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SalesTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                        ProcessedByUserName = x.ProcessedByUser.UserName!,
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

                InventoryLog inventoryLog = new()
                {
                    ProductId = row.ProductId,
                    Quantity = row.Quantity,
                    Date = DateTime.UtcNow,
                    ChangeType = InventoryChangeType.StockOut
                };
                salesTransaction.SalesTransactionItems.Add(salesTransactionItem);
            }

            await _unitOfWork.Repository<SalesTransaction>().AddAsync(salesTransaction);

            await _unitOfWork.SaveChangesAsync();

            return new SalesTransactionsDTO
            {
                Id = salesTransaction.Id,
                TotalAmount = salesTransaction.TotalAmount,
                PaymentMethod = salesTransaction.PaymentMethod,
                ProcessedByUserId = salesTransaction.ProcessedByUserId,
                ProcessedByUserName = salesTransaction.ProcessedByUser.UserName!,
                Date = salesTransaction.Date,
            };
        }

        public async Task<bool> DeleteSalesTransactionAsync(int id)
        {
            var salesTransaction = await GetSalesTransactionAsync(id);
            if (salesTransaction == null)
                return false;

            _unitOfWork.Repository<SalesTransaction>().Delete(salesTransaction);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
        
        public async Task<bool> VoidSalesTransactionAsync(int id, string userId)
        {
            var transaction = await _unitOfWork.Repository<SalesTransaction>().GetQueryable().Include(t => t.SalesTransactionItems)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
            {
                return false; // should return an exception but this is fine for now 
                //return NotFound();
            }    

            if (transaction.IsVoided)
            {
                return false; // should return an exception but this is fine for now 
                //return BadRequest("Transaction is already voided.");
            }
                

            // Mark as voided
            transaction.IsVoided = true;
            transaction.VoidedAt = DateTime.UtcNow;
            transaction.VoidedByUserId = userId;

            // Reverse inventory changes (optional, depends on your system)
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
            return true;
        }
    }
}
