using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Services
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
        Task<SalesTransactionsDTO> CreateSalesTransactionAsync(SalesTransactionCreateInputDTO salesTransactionDTO);
        Task<bool> DeleteSalesTransactionAsync(int id);
    }
}
