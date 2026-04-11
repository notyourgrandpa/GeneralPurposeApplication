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
        private readonly IQueryExecutor _queryExecutor;

        public GetCategoriesHandler(IApplicationDbContext context, IQueryExecutor queryExecutor)
        {
            _queryExecutor = queryExecutor;
        }

        public async Task<PagingResult<CategoryDTO>> Handle(GetCategoriesQuery command, CancellationToken cancellationToken = default)
        {
            return await _queryExecutor.ExecuteAsync<Category, CategoryDTO>(command.Query, x => new CategoryDTO
            {
                Id = x.Id,
                Name = x.Name,
            });
        }
    }
}
