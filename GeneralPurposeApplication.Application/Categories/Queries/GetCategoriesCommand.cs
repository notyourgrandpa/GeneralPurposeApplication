using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Application.Categories.Queries
{
    public class GetCategoriesCommand
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public string? FilterColumn { get; set; }
        public string? FilterQuery { get; set; }
    }
}
