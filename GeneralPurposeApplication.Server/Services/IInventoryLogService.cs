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
    public interface IInventoryLogService
    {
        Task<ApiResult<InventoryLogDTO>> GetInventoryLogsAsync(int pageIndex = 0,
            int pageSize = 10,
            string? sortColumn = null,
            string? sortOrder = null,
            string? filterColumn = null,
            string? filterQuery = null);

        Task<InventoryLog?> GetInventoryLogAsync(int id);
        Task<InventoryLogDTO> CreateInventoryLogAsync(InventoryLogCreateDto inventoryLogDto);
        Task<bool> UpdateInventoryLogAsync(int id, InventoryLogUpdateDTO inventoryLogDto);
        Task<bool> DeleteInventoryLogAsync(int id);

    }
}
