using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.QueryParameters;
using GeneralPurposeApplication.Domain.Expenses;

namespace GeneralPurposeApplication.Application.Services
{
    public interface IExpenseService
    {
        Task<ApiResult<ExpenseDTO>> GetExpensesAsync(QueryParameter parameters);
        Task<Expense> GetExpenseByIdAsync(int id);
        Task<ExpenseDTO> CreateExpenseAsync(ExpenseCreateDTO expenseDTO);
        Task UpdateExpenseAsync(int id, ExpenseUpdateDTO expenseDTO);
        Task DeleteExpenseAsync(int id);
    }
}
