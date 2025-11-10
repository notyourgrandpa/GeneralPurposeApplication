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
                        ProductName = x.Product!.Name
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

            InventoryLog inventoryLog = new()
            {
                ProductId = inventoryLogDto.ProductId,
                Quantity = inventoryLogDto.Quantity,
                ChangeType = inventoryLogDto.ChangeType,
                Remarks = inventoryLogDto.Remarks,
                Date = DateTime.UtcNow
            };

            await _unitOfWork.Repository<InventoryLog>().AddAsync(inventoryLog);

            await _productService.UpdateStockAsync(inventoryLog);

            await _unitOfWork.SaveChangesAsync();

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
    }
}
