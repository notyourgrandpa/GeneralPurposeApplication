using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.QueryParameters;
using GeneralPurposeApplication.Domain.Categories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Categories.Queries
{
    public class GetCategoriesHandler: IRequestHandler<GetCategoriesQuery, PagingResult<CategoryDTO>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IFilterBuilder _filterBuilder;
        private readonly ISortBuilder _sortBuilder;

        public GetCategoriesHandler(IApplicationDbContext context, IFilterBuilder filterBuilder, ISortBuilder sortBuilder)
        {
            _context = context;
            _filterBuilder = filterBuilder;
            _sortBuilder = sortBuilder;
        }

        public async Task<PagingResult<CategoryDTO>> Handle(GetCategoriesQuery command, CancellationToken cancellationToken = default)
        {
            var query = command.Query;
            var categoriesQuery = _context.Categories
                .Include(x => x.Products)
                .AsNoTracking()
                .AsQueryable();

            categoriesQuery = _filterBuilder.Apply(categoriesQuery, query.Filters ?? new List<FilterCondition>());
            categoriesQuery = _sortBuilder.Apply(categoriesQuery, query.SortColumn, query.SortDirection);

            var items = await categoriesQuery
                .Skip((query.PageIndex - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(x => new CategoryDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    TotalProducts = x.Products.Count()
                }).ToListAsync(cancellationToken);

            return new PagingResult<CategoryDTO>(items, query.PageIndex, query.PageSize, items.Count());
        }
    }
}
