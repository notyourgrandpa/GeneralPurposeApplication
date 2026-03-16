using FluentValidation;
using GeneralPurposeApplication.Application.Common.Interfaces;
using GeneralPurposeApplication.Application.DTOs;
using GeneralPurposeApplication.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Commands
{
    public class CreateCategoryHandler
    {
        private readonly IApplicationDbContext _context;
        private readonly CreateCategoryValidator _validator;

        public CreateCategoryHandler(IApplicationDbContext context, CreateCategoryValidator validator)
        {
            _context = context;
            _validator = validator;
        }

        public async Task<Category> Handle(CreateCategoryCommand command)
        {
            await _validator.ValidateAndThrowAsync(command);

            var category = new Category 
            { 
                Name = command.Name 
            };

            _context.Categories.Add(category);

            return category;
        }
    }
}
