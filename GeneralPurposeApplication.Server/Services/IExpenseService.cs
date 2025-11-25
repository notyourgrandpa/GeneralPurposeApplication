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
    public interface IExpenseService
    {
        Task<ApiResult<ExpenseDTO>> GetExpensesAsync(QueryParameter parameters);
        Task<Expense> GetExpenseByIdAsync(int id);
        Task<ExpenseDTO> CreateExpenseAsync(ExpenseCreateDTO expenseDTO);
        Task UpdateExpenseAsync(int id, ExpenseDTO expenseDTO);
        Task DeleteExpenseAsync(int id);
    }
}
