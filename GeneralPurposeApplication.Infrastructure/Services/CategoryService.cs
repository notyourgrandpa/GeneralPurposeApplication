using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.Services;
using GeneralPurposeApplication.Domain.Abstractions;
using GeneralPurposeApplication.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Infrastructure.Services
{
    public class CategoryService: ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork) 
        { 
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsDupeField(int categoryId, string fieldName, string fieldValue)
        {

            if (!ApiResult<Category>.IsValidProperty(fieldName, true))
                return false;

            return await _unitOfWork.Repository<Category>().AnyAsync(
                string.Format("{0} == @0 && Id != @1", fieldName),
                fieldValue,
                categoryId);
        }
    }
}
