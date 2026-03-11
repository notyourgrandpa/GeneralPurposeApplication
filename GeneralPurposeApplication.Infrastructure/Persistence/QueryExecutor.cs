using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Infrastructure.Persistence
{
    public class EfQueryExecutor : IQueryExecutor
    {
        private readonly IApplicationDbContext _context;

        public EfQueryExecutor(IApplicationDbContext context)
        {
            _context = context;
        }

        public Task<PagingResult<TDto>> ExecuteAsync<TEntity,TDto>(PagingQuery query) where TEntity : class
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();
            throw new NotImplementedException();
        }
    }
}
