using GeneralPurposeApplication.Application.QueryParameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Common.Interfaces
{
    public interface IFilterBuilder
    {
        public IQueryable<TEntity> Apply<TEntity>(IQueryable<TEntity> source, List<FilterCondition> filters) where TEntity : class;
    }
}
