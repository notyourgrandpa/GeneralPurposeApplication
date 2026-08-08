using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.QueryParameters;
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
        Task<bool> IsDupeField<T>(int categoryId, string fieldName, string fieldValue) where T : class;
    }
}
