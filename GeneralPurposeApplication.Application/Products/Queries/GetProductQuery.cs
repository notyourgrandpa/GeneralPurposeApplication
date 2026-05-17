using GeneralPurposeApplication.Domain.Products;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Products.Queries
{
    public class GetProductQuery: IRequest<Product>
    {
        public int Id { get; set; }
    }
}
