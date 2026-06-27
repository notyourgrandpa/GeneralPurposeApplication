using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Sales_Transactions.Commands
{
    public class DeleteSalesTransactionCommand : IRequest<Unit>
    {
        public int SalesTransactionId { get; set; }
    }
}
