using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Infrastructure.Persistence
{
    public class QueryExecutor : IQueryExecutor<object, object>
    {
        public Task<PagingResult<object>> ExecuteAsync(PagingQuery query)
        {
            throw new NotImplementedException();
        }
    }
}
