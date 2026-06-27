
using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Domain.Inventory;

namespace GeneralPurposeApplication.Application.Services
{
    public interface IInventoryLogService
    {
        Task<InventoryLogDTO> CreateInventoryLogAsync(InventoryLogCreateDto inventoryLogDto);
        Task UpdateInventoryLogAsync(int id, InventoryLogUpdateDTO inventoryLogDto);
        Task DeleteInventoryLogAsync(int id);
    }
}
