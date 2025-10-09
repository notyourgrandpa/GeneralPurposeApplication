using GeneralPurposeApplication.Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Repositories
{
    public interface IInventoryLogRepository
    {
        public Task<InventoryLog?> GetInventoryLogById(int id);
        public Task<IEnumerable<InventoryLog>> GetAllInventoryLogsAsync();
        public Task AddInventoryLogAsync(InventoryLog log);
        public Task AddInventoryLogsAsync(IEnumerable<InventoryLog> logs);
        public Task UpdateInventoryLogAsync(InventoryLog log);
        public Task DeleteInventoryLogAsync(int id);
    }
}
