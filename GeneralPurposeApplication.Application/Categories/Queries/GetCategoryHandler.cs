using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Domain.Categories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Categories.Queries
{
    public class GetCategoryHandler : IRequestHandler<GetCategoryQuery, Category>
    {
        private readonly IApplicationDbContext _context;

        public GetCategoryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Category> Handle(GetCategoryQuery request, CancellationToken cancellationToken = default)
        {
            Category? category = await _context.Categories.FindAsync(request.Id);

            if (category == null) 
            {
                throw new InvalidOperationException($"Category {request.Id} not found.");
            }

            return category;
        }
    }
}
