using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Common.Interfaces
{
    public interface IQueryExecutor
    {
        Task<PagingResult<TDto>> ExecuteAsync<TEntity, TDto>(PagingQuery query) where TEntity : class;
    }
}
