using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Domain.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Products.Queries
{
    public class GetProductHandler : IRequestHandler<GetProductQuery, Product>
    {
        private readonly IApplicationDbContext _context;

        public GetProductHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> Handle(GetProductQuery request, CancellationToken cancellationToken = default)
        {
            var product = await _context.Products.FirstOrDefaultAsync(x => x.Id == request.Id);

            if(product == null)
            {
                throw new KeyNotFoundException($"Product {request.Id} not found!");
            }
            return product;
        }
    }
}
