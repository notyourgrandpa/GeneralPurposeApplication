
using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Domain.Inventory;

namespace GeneralPurposeApplication.Application.Services
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
        Task UpdateInventoryLogAsync(int id, InventoryLogUpdateDTO inventoryLogDto);
        Task DeleteInventoryLogAsync(int id);
        Task VoidInventoryLogAsync(int id, string userId);

    }
}
