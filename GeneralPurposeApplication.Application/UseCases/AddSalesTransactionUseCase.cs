using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.Services;
using GeneralPurposeApplication.Domain.Abstractions;

namespace GeneralPurposeApplication.Application.UseCases
{
    public class AddSalesTransactionUseCase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISalesTransactionService _salesTransactionService;

        public AddSalesTransactionUseCase(IUnitOfWork unitOfWork, ISalesTransactionService salesTransactionService)
        {
            _unitOfWork = unitOfWork;
            _salesTransactionService = salesTransactionService;
        }

        public async Task<SalesTransactionsDTO> ExecuteAsync(SalesTransactionCreateDTO salesTransactionCreateDTO, string UserId)
        {
            var salesTransaction = await _salesTransactionService.CreateSalesTransactionAsync(salesTransactionCreateDTO, UserId);
            return salesTransaction;
        }
    }
}
