using GeneralPurposeApplication.Server.Data;
using GeneralPurposeApplication.Server.Data.DTOs;
using GeneralPurposeApplication.Server.Data.Models;
using GeneralPurposeApplication.Server.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Services
{
    public class CategoryService: ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResult<CategoryDTO>> GetCategoriesAsync(int pageIndex, int pageSize, string? sortColumn, string? sortOrder, string? filterColumn, string? filterQuery)
        {
            return await ApiResult<CategoryDTO>.CreateAsync(
                _unitOfWork.Repository<Category>().GetQueryable()
                    .AsNoTracking()
                    .Select(x => new CategoryDTO
                    {
                        Id = x.Id,
                        Name = x.Name,
                        TotalProducts = x.Products!.Count
                    }),
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
        }

        public async Task<Category?> GetCategoryAsync(int id) => await _unitOfWork.Repository<Category>().GetByIdAsync(id);

        public async Task<Category> CreateCategoryAsync(CategoryCreateInputDTO categoryCreateDTO)
        {
            Category category = new Category { 
                Name = categoryCreateDTO.Name 
            };

            await _unitOfWork.Repository<Category>().AddAsync(category);
            await _unitOfWork.SaveChangesAsync();

            return category;
        }

        public async Task UpdateCategoryAsync(int id, CategoryUpdateDTO categoryUpdateDTO)
        {
            Category? category = await _unitOfWork.Repository<Category>().GetByIdAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException();
            }
            category.Name = categoryUpdateDTO.Name;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            Category? category = await GetCategoryAsync(id);
            if (category == null)
            {
                throw new KeyNotFoundException();
            }
            _unitOfWork.Repository<Category>().Delete(category);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
