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
    public interface ICategoryService
    {
        Task<ApiResult<CategoryDTO>> GetCategoriesAsync(
            int pageIndex,
            int pageSize,
            string? sortColumn,
            string? sortOrder,
            string? filterColumn,
            string? filterQuery);

        Task<Category?> GetCategoryAsync(int id);
        Task<Category> CreateCategoryAsync(CategoryCreateInputDTO categoryCreateDTO);
        Task UpdateCategoryAsync(int id, CategoryUpdateDTO categoryUpdateDTO);
        Task DeleteCategoryAsync(int id);
        Task CategoryExists(string categoryName);
    }
}
