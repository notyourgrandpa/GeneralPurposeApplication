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
                        Description = x.Description!,
                        Amount = x.Amount,
                        Date = x.Date
                    }), 
                    parameters);
        }

        public async Task<Expense> GetExpenseByIdAsync(int id)
        {
            var expense = await _unitOfWork.Repository<Expense>().GetByIdAsync(id);

            if(expense == null)
            {
                throw new KeyNotFoundException();
            }
            return expense;
        }

        public async Task<ExpenseDTO> CreateExpenseAsync(ExpenseDTO expenseDTO)
        {
            var expense = new Expense
            {
                Category = expenseDTO.Category,
                Description = expenseDTO.Description,
                Amount = expenseDTO.Amount,
                Date = expenseDTO.Date
            };

            await _unitOfWork.Repository<Expense>().AddAsync(expense);
            await _unitOfWork.SaveChangesAsync();
            return expenseDTO;
        }

        public async Task UpdateExpenseAsync(int id, ExpenseDTO expenseDTO)
        {
            var expense = await _unitOfWork.Repository<Expense>().GetByIdAsync(id);
            if(expense == null)
            {
                throw new KeyNotFoundException();
            }

            expense.Category = expenseDTO.Category;
            expense.Description = expenseDTO.Description;
            expense.Amount = expenseDTO.Amount;
            expense.Date = expenseDTO.Date;

            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteExpenseAsync(int id)
        {
            var expense = await _unitOfWork.Repository<Expense>().GetByIdAsync(id);
            if(expense == null)
            {
                throw new KeyNotFoundException();
            }
            _unitOfWork.Repository<Expense>().Delete(expense);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
