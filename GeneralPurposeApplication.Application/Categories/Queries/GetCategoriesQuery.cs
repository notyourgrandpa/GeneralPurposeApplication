using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.QueryParameters;
using MediatR;

namespace GeneralPurposeApplication.Application.Categories.Queries
{
    public class GetCategoriesQuery: IRequest<PagingResult<CategoryDTO>>
    {
        public PagingQuery Query { get; set; } = new();
    }
}
