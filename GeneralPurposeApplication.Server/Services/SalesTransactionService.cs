using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Data.Models;
using GeneralPurposeApplication.Server.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Services
{
    public class SalesTransactionService : ISalesTransactionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SalesTransactionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<SalesTransactionsDTO>> GetSalesTransactionsAsync(int pageIndex, int pageSize, string? sortColumn, string? sortOrder, string? filterColumn, string? filterQuery)
        {
            return await ApiResult<SalesTransactionsDTO>.CreateAsync(
                _unitOfWork.Repository<SalesTransaction>().GetQueryable()
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

        public Task<SalesTransaction?> GetSalesTransactionAsync(int id)
        {
            return _unitOfWork.Repository<SalesTransaction>().GetByIdAsync(id);
        }

        public Task<SalesTransactionsDTO> CreateSalesTransactionAsync(SalesTransactionCreateDTO salesTransactionDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteSalesTransactionAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
