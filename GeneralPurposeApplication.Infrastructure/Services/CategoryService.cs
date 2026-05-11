using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.Services;
using GeneralPurposeApplication.Domain.Abstractions;
using GeneralPurposeApplication.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using static FluentValidation.Validators.PredicateValidator;
using static OfficeOpenXml.ExcelErrorValue;

namespace GeneralPurposeApplication.Infrastructure.Services
{
    public class CategoryService: ICategoryService
    {
        private readonly IApplicationDbContext _context;

        public CategoryService(IApplicationDbContext context) 
        { 
            _context = context;
        }

        public async Task<bool> IsDupeField(int categoryId, string fieldName, string fieldValue)
        {

            if (!ApiResult<Category>.IsValidProperty(fieldName, true))
                return false;

            string predicate = string.Format("{0} == @0 && Id != @1", fieldName);

            object[] values = { fieldValue, categoryId };

            return await _context.Set<T>().Where(predicate, values).AnyAsync();
        }
    }
}
