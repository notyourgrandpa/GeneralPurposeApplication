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
    public class InventoryLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        public InventoryLogService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<InventoryLog> CreateInventoryLog(InventoryLogCreateInputDto inventoryLogDto)
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
            await _unitOfWork.SaveChangesAsync();

            return inventoryLog;
        }
    }
}
