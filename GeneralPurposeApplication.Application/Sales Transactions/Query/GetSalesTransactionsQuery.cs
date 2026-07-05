using GeneralPurposeApplication.Application.Common.Paging;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Application.QueryParameters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Sales_Transactions.Query
{
    public class GetSalesTransactionsQuery : IRequest<PagedResult<SalesTransactionsDTO>>
    {
        public required QueryParameter Parameter { get; set; }
    }
}
