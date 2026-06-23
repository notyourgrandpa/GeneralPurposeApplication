using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.Products.Commands;
using GeneralPurposeApplication.Application.Services;
using GeneralPurposeApplication.Application.UseCases;
using GeneralPurposeApplication.Domain.Inventory;
using GeneralPurposeApplication.Infrastructure.Persistence.Extensions;
using MediatR;
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
        private readonly IInventoryLogService _inventoryLogService;
        private readonly AddStockUseCase _addStockUseCase;
        private readonly IMediator _mediator;

        public InventoryLogsController(IInventoryLogService inventoryLogService, AddStockUseCase addStockUseCase, IMediator mediator)
        {
            _inventoryLogService = inventoryLogService;
            _addStockUseCase = addStockUseCase;
            _mediator = mediator;
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
            return await _inventoryLogService.GetInventoryLogsAsync(pageIndex, pageSize, sortColumn, sortOrder, filterColumn, filterQuery);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InventoryLog>> GetInventoryLogAsync(int id)
        {
            var inventoryLog = await _inventoryLogService.GetInventoryLogAsync(id);

            if (inventoryLog == null)
            {
                return NotFound();
            }

            return inventoryLog;
        }

        [HttpPost]
        public async Task<ActionResult<InventoryLogDTO>> CreateInventoryLogAsync(InventoryLogCreateDto inventoryLogDto)
        {
            InventoryLogDTO inventoryLog = await _addStockUseCase.ExecuteAsync(inventoryLogDto);

            return CreatedAtAction("GetInventoryLogAsync", new { id = inventoryLog.Id }, inventoryLog);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> VoidInventoryLogAsync(int id)
        {
            await _mediator.Send(new VoidInventoryLogCommand { Id = id, UserId = User.GetUserId() });
            return NoContent();
        }
    }
}
