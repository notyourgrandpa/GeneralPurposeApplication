using GeneralPurposeApplication.Domain.Categories;
using GeneralPurposeApplication.Domain.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Common.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<bool> CategoryExists(string categoryName, CancellationToken ct);
    }
}
