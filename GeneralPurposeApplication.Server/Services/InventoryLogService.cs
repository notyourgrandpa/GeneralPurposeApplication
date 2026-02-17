using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Data.Models;
using GeneralPurposeApplication.Server.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Services
{
    public class InventoryLogService: IInventoryLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;
        public InventoryLogService(IUnitOfWork unitOfWork, IProductService productService)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
        }

        public async Task<ApiResult<InventoryLogDTO>> GetInventoryLogsAsync(
            int pageIndex = 0,
            int pageSize = 10,
            string? sortColumn = null,
            string? sortOrder = null,
            string? filterColumn = null,
            string? filterQuery = null)
        {

            return await ApiResult<InventoryLogDTO>.CreateAsync(

                _unitOfWork.Repository<InventoryLog>().GetQueryable()
                    .Select(x => new InventoryLogDTO
                    {
                        Id = x.Id,
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        ChangeType = x.ChangeType,
                        Remarks = x.Remarks,
                        Date = x.Date,
                        ProductName = x.Product!.Name,
                        IsVoided = x.IsVoided
                    }),
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
        }

        public async Task<InventoryLog?> GetInventoryLogAsync(int id)
        {
            return await _unitOfWork.Repository<InventoryLog>().GetByIdAsync(id);
        }

        public async Task<InventoryLogDTO> CreateInventoryLogAsync(InventoryLogCreateDto inventoryLogDto)
        {
            if (inventoryLogDto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var product = await _productService.GetProductAsync(inventoryLogDto.ProductId);

            InventoryLog inventoryLog = new()
            {
                ProductId = inventoryLogDto.ProductId,
                Quantity = inventoryLogDto.Quantity,
                ChangeType = inventoryLogDto.ChangeType,
                Remarks = inventoryLogDto.Remarks,
                OldStock = product?.Stock ?? 0,
                Date = DateTime.UtcNow
            };

            await _unitOfWork.Repository<InventoryLog>().AddAsync(inventoryLog);

            await _productService.UpdateStockAsync(inventoryLog);

            return new InventoryLogDTO
            {
                Id = inventoryLog.Id,
                ProductId = inventoryLog.ProductId,
                Quantity = inventoryLog.Quantity,
                ChangeType = inventoryLog.ChangeType,
                Remarks = inventoryLog.Remarks,
                Date = inventoryLog.Date
            };
        }

        public async Task UpdateInventoryLogAsync(int id, InventoryLogUpdateDTO inventoryLogDTO)
        {
            InventoryLog? inventoryLog = await _unitOfWork.Repository<InventoryLog>().GetByIdAsync(id);
            if (inventoryLog == null)
                throw new KeyNotFoundException();

            inventoryLog.ProductId = inventoryLogDTO.ProductId;
            inventoryLog.Quantity = inventoryLogDTO.Quantity;
            inventoryLog.ChangeType = inventoryLog.ChangeType;
            inventoryLog.Remarks = inventoryLogDTO.Remarks;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteInventoryLogAsync(int id)
        {
            InventoryLog? inventoryLog = await _unitOfWork.Repository<InventoryLog>().GetByIdAsync(id);
            if (inventoryLog == null)
                throw new KeyNotFoundException();

            _unitOfWork.Repository<InventoryLog>().Delete(inventoryLog);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task VoidInventoryLogAsync(int inventoryLogId, string userId)
        {
            var inventoryLogRepo = _unitOfWork.Repository<InventoryLog>();

            var inventoryLog = await inventoryLogRepo
                .GetByIdAsync(inventoryLogId, q => q.Product!);

            if (inventoryLog == null)
                throw new KeyNotFoundException($"Inventory Log {inventoryLogId} not found.");

            if (inventoryLog.IsVoided)
                throw new InvalidOperationException("This inventory log is already voided.");

            // Void the log
            inventoryLog.IsVoided = true;
            inventoryLog.VoidedAt = DateTime.UtcNow;
            inventoryLog.VoidedByUserId = userId;

            var logs = await inventoryLogRepo.GetAllAsync(
                l => l.ProductId == inventoryLog.ProductId && l.Id != inventoryLogId,
                orderBy: q => q
                    .OrderBy(l => l.Date)
                    .ThenBy(l => l.Id)
            );

            int runningStock = 0;

            foreach (var log in logs)
            {
                if (log.IsVoided)
                    continue;

                log.OldStock = runningStock;

                switch (log.ChangeType)
                {
                    case InventoryChangeType.StockIn:
                        runningStock += log.Quantity;
                        break;

                    case InventoryChangeType.StockOut:
                        runningStock -= log.Quantity;
                        break;

                    case InventoryChangeType.Adjustment:
                        runningStock = log.Quantity; // absolute adjustment
                        break;

                    default:
                        throw new InvalidOperationException("Unknown inventory change type.");
                }
            }

            inventoryLog.Product!.Stock = runningStock;

            await _unitOfWork.SaveChangesAsync();
        }

    }
}
