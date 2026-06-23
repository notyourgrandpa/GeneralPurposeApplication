using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.Services;
using GeneralPurposeApplication.Domain.Abstractions;
using GeneralPurposeApplication.Domain.Inventory;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Inventory_Logs.Commands
{
    public class CreateInventoryLogHandler : IRequestHandler<CreateInventoryLogCommand, InventoryLogDTO>
    {
        private readonly IApplicationDbContext _context;
        private readonly IProductService _productService;

        public CreateInventoryLogHandler(IApplicationDbContext context, IProductService productService)
        {
            _context = context;
            _productService = productService;
        }

        public async Task<InventoryLogDTO> Handle(CreateInventoryLogCommand request, CancellationToken cancellationToken)
        {
            var inventoryLogDto = request.InventoryLogDto;
            if (inventoryLogDto.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            var product = await _context.Products.FindAsync(inventoryLogDto.ProductId);

            InventoryLog inventoryLog = new()
            {
                ProductId = inventoryLogDto.ProductId,
                Quantity = inventoryLogDto.Quantity,
                ChangeType = inventoryLogDto.ChangeType,
                Remarks = inventoryLogDto.Remarks,
                OldStock = product?.Stock ?? 0,
                Date = DateTime.UtcNow
            };

            await _context.InventoryLogs.AddAsync(inventoryLog);

            await _productService.UpdateStockAsync(inventoryLog);

            await _context.SaveChangesAsync();

            return new InventoryLogDTO
            {
                Id = inventoryLog.Id,
                ProductId = inventoryLog.ProductId,
                Quantity = inventoryLog.Quantity,
                ChangeType = inventoryLog.ChangeType,
                Remarks = inventoryLog.Remarks,
                Date = inventoryLog.Date
            };
        }
    }
}
