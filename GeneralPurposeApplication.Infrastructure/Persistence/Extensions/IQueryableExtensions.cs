using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Extensions
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> WhereStartsWith<T> (this IQueryable<T> source, string propertyName, string filterValue)
        {
            var parameter = Expression.Parameter(typeof (T), "p");
            var property = Expression.Property(parameter, propertyName);
            var constant = Expression.Constant(filterValue);
            var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            var body = Expression.Call(property, startsWithMethod!, constant);

            var predicate = Expression.Lambda<Func<T, bool>>(body, parameter);

            return source.Where(predicate);
        }
    }
}
