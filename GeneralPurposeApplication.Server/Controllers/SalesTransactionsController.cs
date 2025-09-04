using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Data.Models;
using GeneralPurposeApplication.Server.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeneralPurposeApplication.Server.Controllers
{
    public class SalesTransactionsController: ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SalesTransactionsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // GET: api/SalesTransactions
        // GET: api/SalesTransactions/?pageIndex=0&pageSize=10
        // GET: api/SalesTransactions/?pageIndex=0&pageSize=10&sortColumn=name&sortOrder=asc
        [HttpGet]
        public async Task<ApiResult<SalesTransactionsDTO>> GetSalesTransactions(
            int pageIndex = 0,
            int pageSize = 10,
            string? sortColumn = null,
            string? sortOrder = null,
            string? filterColumn = null,
            string? filterQuery = null)
        {
            return await ApiResult<SalesTransactionsDTO>.CreateAsync(
                _context.SalesTransactions
                    .AsNoTracking()
                    .Select(x => new SalesTransactionsDTO
                    {
                        Id = x.Id,
                        TotalAmount = x.TotalAmount,
                        PaymentMethod = x.PaymentMethod,
                        ProcessedByUserId = x.ProcessedByUserId,
                        ProcessedByUserName = x.ProcessedByUser.UserName!,
                        Date = x.Date,
                    }),
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SalesTransaction>> GetSalesTransactionAsync(int id)
        {
            var inventoryLog = await _context.SalesTransactions.FindAsync(id);

            if (inventoryLog == null)
            {
                return NotFound();
            }

            return inventoryLog;
        }

        //[HttpPost]
        //public async Task<ActionResult<SalesTransaction>> CreateSalesTransaction(SalesTransactionCreateInputDTO salesTransactionLogDto)
        //{

        //    //if (salesTransactionLogDto.Quantity <= 0)
        //    //    return BadRequest("Quantity must be greater than zero.");

        //    using var transaction = await _context.Database.BeginTransactionAsync();

        //    try
        //    {
        //        var salesTransaction = new SalesTransaction
        //        {
        //            PaymentMethod = salesTransactionLogDto.PaymentMethod,
        //            ProcessedByUserId = User.GetUserId(),
        //            Date = DateTime.UtcNow
        //        };

        //        _context.SalesTransactions.Add(salesTransaction);

        //        var product = await _context.Products.FindAsync(salesTransaction.ProductId);

        //        if (product == null)
        //            return NotFound($"Product {salesTransaction.ProductId} not found.");

        //        if (salesTransaction.ChangeType == InventoryChangeType.StockIn)
        //            product.Stock += salesTransaction.Quantity;
        //        else if (salesTransaction.ChangeType == InventoryChangeType.StockOut)
        //        {
        //            if (product.Stock < salesTransaction.Quantity)
        //                throw new InvalidOperationException("Not enough stock available.");
        //            product.Stock -= salesTransaction.Quantity;
        //        }
        //        else if (salesTransaction.ChangeType == InventoryChangeType.Adjustment)
        //            product.Stock = salesTransaction.Quantity;

        //        product.LastUpdated = DateTime.Now;

        //        await _context.SaveChangesAsync();

        //        await transaction.CommitAsync();

        //        return CreatedAtAction("GetInventoryLog", new { id = salesTransaction.Id }, new SalesTransactionsDTO
        //        {
        //            Id = salesTransaction.Id,
        //            //ProductId = salesTransaction.ProductId,
        //            //Quantity = salesTransaction.Quantity,
        //            //ChangeType = salesTransaction.ChangeType,
        //            //Remarks = salesTransaction.Remarks
        //        });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        await transaction.RollbackAsync();
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        //[HttpPut]
        //public async Task<IActionResult> PutInventory(int id, InventoryLogUpdateInputDTO salesTransactionLogDto)
        //{
        //    if (id != salesTransactionLogDto.Id)
        //        return BadRequest();

        //    var inventoryLog = await _context.SalesTransactions.FindAsync(id);
        //    if (inventoryLog == null)
        //        return NotFound();

        //    inventoryLog.ProductId = inventoryLogDto.ProductId;
        //    inventoryLog.Quantity = inventoryLogDto.Quantity;
        //    inventoryLog.ChangeType = inventoryLogDto.ChangeType;
        //    inventoryLog.Remarks = inventoryLogDto.Remarks;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        throw;

        //    }
        //    return NoContent();
        //}

        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteInvetoryLog(int id)
        //{
        //    var inventoryLog = await _context.SalesTransactions.FindAsync(id);

        //    if (inventoryLog == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.SalesTransactions.Remove(inventoryLog);
        //    await _context.SaveChangesAsync();

        //    return NoContent();

        //}
    }
}
