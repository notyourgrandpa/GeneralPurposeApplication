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
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}
