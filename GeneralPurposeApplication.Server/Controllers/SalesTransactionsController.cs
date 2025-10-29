using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Data.Models;
using GeneralPurposeApplication.Server.Extensions;
using GeneralPurposeApplication.Server.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GeneralPurposeApplication.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesTransactionsController: ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISalesTransactionService _salesTransactionService;

        public SalesTransactionsController(ApplicationDbContext context, ISalesTransactionService salesTransactionService)
        {
            _context = context;
            _salesTransactionService = salesTransactionService;
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

        [HttpPost]
        public async Task<ActionResult<SalesTransaction>> CreateSalesTransactionAsync(SalesTransactionCreateDTO salesTransactionLogDto)
        {
            try
            {
                var salesTransactionDto = await _salesTransactionService.CreateSalesTransactionAsync(salesTransactionLogDto, User.GetUserId());

                return CreatedAtAction("GetSalesTransaction", new { id = salesTransactionDto.Id }, salesTransactionDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
    }

        //[HttpPut]
        //public async Task<IActionResult> PutInventory(int id, SalesTransactionUpdateInputDTO salesTransactionLogDto)
        //{
        //    if (id != salesTransactionLogDto.Id)
        //        return BadRequest();

        //    var salesTransaction = await _context.SalesTransactions.FindAsync(id);
        //    if (salesTransaction == null)
        //        return NotFound();

        //    salesTransaction.ProductId = salesTransactionDto.ProductId;
        //    salesTransaction.Quantity = salesTransactionDto.Quantity;
        //    salesTransaction.ChangeType = salesTransactionDto.ChangeType;
        //    salesTransaction.Remarks = salesTransactionDto.Remarks;

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


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesTransaction(int id)
        {
            var inventoryLog = await _context.SalesTransactions.FindAsync(id);

            if (inventoryLog == null)
            {
                return NotFound();
            }

            _context.SalesTransactions.Remove(inventoryLog);
            await _context.SaveChangesAsync();

            return NoContent();

        }

        [HttpPost("{id}/void")]
        public async Task<IActionResult> VoidSalesTransaction(int id)
        {
            var transaction = await _context.SalesTransactions
                .Include(t => t.SalesTransactionItems)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transaction == null)
                return NotFound();

            if (transaction.IsVoided)
                return BadRequest("Transaction is already voided.");

            // Mark as voided
            transaction.IsVoided = true;
            transaction.VoidedAt = DateTime.UtcNow;
            transaction.VoidedByUserId = User.GetUserId();

            // Reverse inventory changes (optional, depends on your system)
            foreach (var item in transaction.SalesTransactionItems)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product != null)
                {
                    product.Stock += item.Quantity; // restore stock
                    product.LastUpdated = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Transaction voided successfully." });
        }
    }
}
