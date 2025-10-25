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
        Task<ApiResult<SalesTransactionsDTO>> GetSalesTransactionsAsync();
        Task<SalesTransaction?> GetSalesTransactionAsync();
        Task<SalesTransactionsDTO> CreateSalesTransactionAsync();
        Task<bool> UpdateSalesTransactionAsync();
        Task<bool> DeleteSalesTransactionAsync();
    }
}
