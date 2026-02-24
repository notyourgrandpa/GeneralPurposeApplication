using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.DTOs
{
    public class CategoryUpdateDTO: CategoryCreateInputDTO
    {
        public int Id { get; set; }
    }
}
