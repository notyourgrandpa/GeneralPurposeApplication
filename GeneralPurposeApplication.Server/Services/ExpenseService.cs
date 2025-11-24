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
    public class ExpenseService : IExpenseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResult<ExpenseDTO>> GetExpensesAsync(QueryParameter parameters)
        {
            return await ApiResult<ExpenseDTO>.CreateAsync(
                _unitOfWork.Repository<Expense>()
                    .GetQueryable()
                    .Select(x => new ExpenseDTO
                    {
                        Id = x.Id,
                        Category = x.Category,
                        Description = x.Description,
                        Amount = x.Amount,
                        Date = x.Date
                    }), 
                    parameters);
        }

        public Task<Expense> GetExpenseByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ExpenseDTO> CreateExpenseAsync(ExpenseDTO expenseDTO)
        {
            throw new NotImplementedException();
        }

        public Task UpdateExpenseAsync(int id, ExpenseDTO expenseDTO)
        {
            throw new NotImplementedException();
        }

        public Task DeleteExpenseAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
