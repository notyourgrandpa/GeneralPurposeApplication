using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Domain.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Services
{
    public interface ISalesTransactionService
    {
        Task<ApiResult<SalesTransactionsDTO>> GetSalesTransactionsAsync(
            int pageIndex,
            int pageSize,
            string? sortColumn,
            string? sortOrder,
            string? filterColumn,
            string? filterQuery);
        Task<SalesTransaction?> GetSalesTransactionAsync(int id);
        Task<SalesTransactionsDTO> CreateSalesTransactionAsync(SalesTransactionCreateDTO salesTransactionDTO, string userID);
        Task DeleteSalesTransactionAsync(int id);
        Task VoidSalesTransactionAsync(int id, string userId);
    }
}
