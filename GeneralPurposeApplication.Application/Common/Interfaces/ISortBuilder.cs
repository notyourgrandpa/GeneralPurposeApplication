using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Common.Interfaces
{
    public interface ISortBuilder
    {
        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> source, string? sortColumn, string? sortDirection) where TEntity : class;
    }
}
