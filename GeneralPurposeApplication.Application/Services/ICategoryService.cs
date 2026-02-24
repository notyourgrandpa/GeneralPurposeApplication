using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Services
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
        Task<bool> CategoryExists(string categoryName);
        Task<bool> IsDupeField(int categoryId, string fieldName, string fieldValue);
    }
}
