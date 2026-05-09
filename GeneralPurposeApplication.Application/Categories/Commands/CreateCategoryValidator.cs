using FluentValidation;
using GeneralPurposeApplication.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Categories.Commands
{
    public class CreateCategoryValidator: AbstractValidator<CreateCategoryCommand>
    {
        public CreateCategoryValidator(ICategoryRepository repository)
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MustAsync(async (name, ct) => 
                    !await repository.CategoryExists(name, ct))
                .WithMessage("Category already exist!");
        }
    }
}
