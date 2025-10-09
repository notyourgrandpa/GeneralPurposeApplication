using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Repositories
{
    public class InventoryLogRepository : IInventoryLogRepository
    {
        private ApplicationDbContext _context { get; set; }

        public InventoryLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InventoryLog?> GetInventoryLogById(int id)
        {
            return await _context.InventoryLogs.FindAsync(id);
        }

        public async Task<IEnumerable<InventoryLog>> GetAllInventoryLogsAsync()
        {
            return await _context.InventoryLogs
                .AsNoTracking()
                .ToListAsync();
        }   

        public async Task AddInventoryLogAsync(InventoryLog log)
        {
            _context.InventoryLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddInventoryLogsAsync(IEnumerable<InventoryLog> logs)
        {
            _context.InventoryLogs.AddRange(logs);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateInventoryLogAsync(InventoryLog log)
        {
            _context.InventoryLogs.Update(log);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInventoryLogAsync(int id)
        {
            InventoryLog inventoryLog = await GetInventoryLogById(id)
                ?? throw new KeyNotFoundException($"Inventory log with ID {id} not found.");
            
            _context.InventoryLogs.Remove(inventoryLog);
            await _context.SaveChangesAsync();
        }
    }
}
