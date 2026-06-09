using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.QueryParameters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Products.Queries
{
    public class GetProductsQuery: IRequest<PagedResult<ProductDTO>>
    {
        public required ProductQueryParameter ProductQuery;
    }
}
