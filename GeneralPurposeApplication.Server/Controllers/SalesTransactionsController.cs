using GeneralPurposeApplication.Server.Application.UseCases;
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
        private readonly ISalesTransactionService _salesTransactionService;
        private readonly AddSalesTransactionUseCase _addSalesTransactionUseCase;

        public SalesTransactionsController(ISalesTransactionService salesTransactionService, AddSalesTransactionUseCase addSalesTransactionUseCase)
        {
            _salesTransactionService = salesTransactionService;
            _addSalesTransactionUseCase = addSalesTransactionUseCase;
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
            return await _salesTransactionService.GetSalesTransactionsAsync(
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
            var salesTransaction = await _salesTransactionService.GetSalesTransactionAsync(id);
            if (salesTransaction == null)
            {
                return NotFound();
            }
            return salesTransaction;
        }

        [HttpPost]
        public async Task<ActionResult<SalesTransaction>> CreateSalesTransactionAsync(SalesTransactionCreateDTO salesTransactionLogDto)
        {
            var salesTransactionDto = await _addSalesTransactionUseCase.ExecuteAsync(salesTransactionLogDto, User.GetUserId());

            return CreatedAtAction("GetSalesTransaction", new { id = salesTransactionDto.Id }, salesTransactionDto);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesTransactionAsync(int id)
        {
            await _salesTransactionService.DeleteSalesTransactionAsync(id);

            return NoContent();
        }

        [HttpPost("{id}/void")]
        public async Task<IActionResult> VoidSalesTransaction(int id)
        {
            await _salesTransactionService.VoidSalesTransactionAsync(id, User.GetUserId());

            return NoContent();
        }
    }
}
