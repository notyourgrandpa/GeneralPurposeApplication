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
        //Task<IEnumerable<InventoryLogDTO>> GetInventoryLogsAsync();
        //Task<InventoryLog> GetInventoryLogAsync(int id);
        Task<InventoryLog> CreateInventoryLogAsync(InventoryLogCreateDto inventoryLogDto);
        //Task<InventoryLog> UpdateInventoryLogAsync(int id, InventoryLogUpdateInputDTO inventoryLogDto);
        //Task<bool> DeleteInventoryLogAsync(int id);

    }
}
