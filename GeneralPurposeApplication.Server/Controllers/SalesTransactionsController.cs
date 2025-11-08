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
            var salesTransactionDto = await _salesTransactionService.CreateSalesTransactionAsync(salesTransactionLogDto, User.GetUserId());

            return CreatedAtAction("GetSalesTransaction", new { id = salesTransactionDto.Id }, salesTransactionDto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesTransactionAsync(int id)
        {
            var inventoryLog = await _salesTransactionService.DeleteSalesTransactionAsync(id);

            if (!inventoryLog)
            {
                return NotFound();
            }

            return NoContent();

        }

        [HttpPost("{id}/void")]
        public async Task<IActionResult> VoidSalesTransaction(int id)
        {
            var isVoided = await _salesTransactionService.VoidSalesTransactionAsync(id, User.GetUserId());
            if (!isVoided)
            {
                return NotFound();

            }

            return NoContent();
        }
    }
}
