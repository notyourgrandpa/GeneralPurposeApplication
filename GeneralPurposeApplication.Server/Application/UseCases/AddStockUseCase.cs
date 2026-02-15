using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Repositories;
using GeneralPurposeApplication.Server.Services;

namespace GeneralPurposeApplication.Server.Application.UseCases
{
    public class AddStockUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInventoryLogService _inventoryLogService;

        public AddStockUseCase(IUnitOfWork unitOfWork, IInventoryLogService inventoryLogService)
        {
            _unitOfWork = unitOfWork;
            _inventoryLogService = inventoryLogService;
        }

        public async Task<InventoryLogDTO> ExecuteAsync(InventoryLogCreateDto inventoryLogCreateDto)
        {
            InventoryLogDTO inventoryLog = await _inventoryLogService.CreateInventoryLogAsync(inventoryLogCreateDto);
            await _unitOfWork.SaveChangesAsync();
            return inventoryLog;
        }
    }
}
