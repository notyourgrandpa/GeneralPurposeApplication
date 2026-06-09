using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Domain.Products;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using GeneralPurposeApplication.Application.Common.Paging;

namespace GeneralPurposeApplication.Application.Products.Queries
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, Common.Paging.PagedResult<ProductDTO>>
    {
        private readonly IApplicationDbContext _context;

        public GetProductsHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Common.Paging.PagedResult<ProductDTO>> Handle(
    GetProductsQuery request,
    CancellationToken cancellationToken)
        {
            var param = request.ProductQuery;

            IQueryable<Product> query = _context.Products
                .Include(p => p.Category)
                .AsNoTracking();

            // Filtering

            if (param.CategoryId.HasValue)
            {
                query = query.Where(p =>
                    p.CategoryId == param.CategoryId);
            }

            if (!string.IsNullOrWhiteSpace(param.FilterQuery))
            {
                query = query.Where(p =>
                    p.Name.Contains(param.FilterQuery));
            }

            if (param.IsActive.HasValue)
            {
                query = query.Where(p =>
                    p.IsActive == param.IsActive);
            }

            // Sorting

            query = (param.SortColumn, param.SortOrder?.ToLower()) switch
            {
                ("name", "desc") =>
                    query.OrderByDescending(p => p.Name),

                ("name", _) =>
                    query.OrderBy(p => p.Name),

                ("costPrice", "desc") =>
                    query.OrderByDescending(p => p.CostPrice),

                ("costPrice", _) =>
                    query.OrderBy(p => p.CostPrice),

                ("Stock", "desc") =>
                    query.OrderByDescending(p => p.Stock),

                ("stock", _) =>
                    query.OrderBy(p => p.Stock),

                _ =>
                    query.OrderBy(p => p.Id)
            };

            var totalCount = await query.CountAsync(cancellationToken);

            var data = await query
                .Skip(param.PageIndex * param.PageSize)
                .Take(param.PageSize)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category!.Name,
                    CostPrice = p.CostPrice,
                    SellingPrice = p.SellingPrice,
                    Stock = p.Stock,
                    IsActive = p.IsActive,
                    DateAdded = p.DateAdded,
                    LastUpdated = p.LastUpdated
                })
                .ToListAsync(cancellationToken);

            return new Common.Paging.PagedResult<ProductDTO>(
                data,
                totalCount,
                param.PageIndex,
                param.PageSize);
        }
    }
}
