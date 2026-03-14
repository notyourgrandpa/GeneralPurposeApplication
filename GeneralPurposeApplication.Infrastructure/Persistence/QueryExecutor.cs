using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.QueryParameters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
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
            IQueryable<TEntity> source = _context.Set<TEntity>().AsNoTracking();

            source = ApplyFilters(source, query.Filters ?? new List<FilterCondition>());


            throw new NotImplementedException();
        }

        private IQueryable<TEntity> ApplyFilters<TEntity>(IQueryable<TEntity> source, List<FilterCondition> filters)
        {
            var param = Expression.Parameter(typeof(TEntity), "x");

            Expression ? combinedExpression = null;
            foreach (FilterCondition filter in filters) 
            { 
                Expression property = Expression.PropertyOrField(param, filter.Field);
                Expression constant = Expression.Constant(Convert.ChangeType(filter.Value, property.Type));

                Expression? condition = filter.Operator switch
                {
                    FilterOperator.Contains => Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, constant),
                    FilterOperator.StartsWith => Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!, constant),
                    FilterOperator.Equals => Expression.Equal(property, constant),
                    FilterOperator.GreaterThan => Expression.GreaterThan(property, constant),
                    FilterOperator.LessThan => Expression.LessThan(property, constant),
                    _ => null,
                };

                if (condition != null)
                {
                    combinedExpression = combinedExpression == null ? condition : Expression.AndAlso(combinedExpression, condition);
                }
            }

            if(combinedExpression == null)
            {
                return source;
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(combinedExpression, param);

            return source.Where(lambda);
        }
    }
}
