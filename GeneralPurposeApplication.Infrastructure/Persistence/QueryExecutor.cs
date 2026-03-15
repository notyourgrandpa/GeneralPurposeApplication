using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.QueryParameters;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using OfficeOpenXml.Utils;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
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

        public async Task<PagingResult<TDto>> ExecuteAsync<TEntity,TDto>(PagingQuery query, Expression<Func<TEntity, TDto>> selector) where TEntity : class
        {
            IQueryable<TEntity> source = _context.Set<TEntity>().AsNoTracking();

            source = ApplyFilters(source, query.Filters ?? new List<FilterCondition>());
            source = ApplySorting(source, query.SortColumn, query.SortDirection);

            var count = await source.CountAsync();

            var pageData = await source
                .Skip(query.PageIndex * query.PageSize)
                .Take(query.PageSize)
                .Select(selector)
                .ToListAsync();


            return new PagingResult<TDto>(
                pageData,
                query.PageIndex,
                query.PageSize,
                count
            );
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

        private IQueryable<TEntity> ApplySorting<TEntity> (IQueryable<TEntity> source, string? sortColumn, string? sortDirection) where TEntity : class
        {
            if (sortColumn == null)
                return source;
            var param = Expression.Parameter(typeof(TEntity), "x");

            Expression property = Expression.PropertyOrField(param, sortColumn);

            var lambda = Expression.Lambda(property, param);

            string methodName =
                sortDirection?.ToUpper() == "DESC"
                ? "OrderByDescending"
                : "OrderBy";

            var resultExpression = Expression.Call(
                typeof(Queryable),
                methodName,
                new Type[] { typeof(TEntity), property.Type },
                source.Expression,
                Expression.Quote(lambda));

            return source.Provider.CreateQuery<TEntity>(resultExpression);
        }
    }
}
