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

        public Task<ApiResult<SalesTransactionsDTO>> GetSalesTransactionsAsync(int pageIndex, int pageSize, string? sortColumn, string? sortOrder, string? filterColumn, string? filterQuery)
        {
            throw new NotImplementedException();
        }

        public Task<SalesTransaction?> GetSalesTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<SalesTransactionsDTO> CreateSalesTransactionAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateSalesTransactionAsync()
        {
            throw new NotImplementedException();
        }


        public Task<bool> DeleteSalesTransactionAsync()
        {
            throw new NotImplementedException();
        }
    }
}
