using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Data.Models;
using GeneralPurposeApplication.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryLogsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly InventoryLogService _inventoryLogService;

        public InventoryLogsController(ApplicationDbContext context, InventoryLogService inventoryLogService)
        {
            _context = context;
            _inventoryLogService = inventoryLogService;
        }
        // GET: api/InventoryLogs
        // GET: api/InventoryLogs/?pageIndex=0&pageSize=10
        // GET: api/InventoryLogs/?pageIndex=0&pageSize=10&sortColumn=name&sortOrder=asc
        [HttpGet]
        public async Task<ApiResult<InventoryLogDTO>> GetInventoryLogs(
            int pageIndex = 0,
            int pageSize = 10,
            string? sortColumn = null,
            string? sortOrder = null,
            string? filterColumn = null,
            string? filterQuery = null)
        {
            return await ApiResult<InventoryLogDTO>.CreateAsync(
                _context.InventoryLogs
                    .AsNoTracking()
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

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryLog>> GetInventoryLogAsync(int id)
        {
            var inventoryLog = await _context.InventoryLogs.FindAsync(id);

            if (inventoryLog == null)
            {
                return NotFound();
            }

            return inventoryLog;
        }

        [HttpPost]
        public async Task<ActionResult<InventoryLog>> CreateInventoryLog(InventoryLogCreateInputDto inventoryLogDto)
        {
            try
            {
                InventoryLog inventoryLog = await _inventoryLogService.CreateInventoryLog(inventoryLogDto);

                return CreatedAtAction("GetInventoryLog", new { id = inventoryLog.Id }, new InventoryLogDTO
                {
                    Id = inventoryLog.Id,
                    ProductId = inventoryLog.ProductId,
                    Quantity = inventoryLog.Quantity,
                    ChangeType = inventoryLog.ChangeType,
                    Remarks = inventoryLog.Remarks
                });
            }
            catch(InvalidOperationException ex)
            {
                //await transaction.RollbackAsync();
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> PutInventory(int id, InventoryLogUpdateInputDTO inventoryLogDto)
        {
            if (id != inventoryLogDto.Id)
                return BadRequest();

            var inventoryLog = await _context.InventoryLogs.FindAsync(id);
            if (inventoryLog == null)
                return NotFound();

            inventoryLog.ProductId = inventoryLogDto.ProductId;
            inventoryLog.Quantity = inventoryLogDto.Quantity;
            inventoryLog.ChangeType = inventoryLogDto.ChangeType;
            inventoryLog.Remarks = inventoryLogDto.Remarks;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                    throw;

            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvetoryLog(int id)
        {
            var inventoryLog = await _context.InventoryLogs.FindAsync(id);

            if(inventoryLog == null)
            {
                return NotFound();
            }

            _context.InventoryLogs.Remove(inventoryLog);
            await _context.SaveChangesAsync();

            return NoContent();

        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(x => x.Id == id);
        }
    }
}
