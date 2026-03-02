using GeneralPurposeApplication.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Common.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<Dictionary<string, Category>> GetDictionaryAsync();
    }
}
