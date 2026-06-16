using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Domain.Products;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Products.Commands
{
    internal class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductDTO>
    {
        private readonly IApplicationDbContext _context;

        public CreateProductHandler(IApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<ProductDTO> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            Product product = new Product
            {
                Name = request.ProductCreateDTO.Name,
                CategoryId = request.ProductCreateDTO.CategoryId,
                CostPrice = request.ProductCreateDTO.CostPrice,
                SellingPrice = request.ProductCreateDTO.SellingPrice,
                Stock = 0,
                IsActive = request.ProductCreateDTO.IsActive,
            };
            product.SetCreated(DateTime.UtcNow);
            product.SetUpdated(DateTime.UtcNow);

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                CostPrice = product.CostPrice,
                SellingPrice = product.SellingPrice,
                Stock = product.Stock,
                IsActive = product.IsActive,
                DateAdded = product.DateAdded,
                LastUpdated = product.LastUpdated
            };
        }
    }
}
