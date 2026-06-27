using GeneralPurposeApplication.Domain.Inventory;
using GeneralPurposeApplication.Domain.Sales;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Sales_Transactions.Query
{
    public class GetSalesTransactionQuery : IRequest<SalesTransaction>
    {
        public int SalesTransactionId { get; set;}
    }
}
