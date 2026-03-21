using GeneralPurposeApplication.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Commands
{ 
    public record CreateCategoryCommand(string Name): IRequest<CategoryDTO> { }
}
