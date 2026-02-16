using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Extensions;
using GeneralPurposeApplication.Server.Repositories;
using GeneralPurposeApplication.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Application.UseCases
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
            await _unitOfWork.SaveChangesAsync();
            return salesTransaction;
        }
    }
}
