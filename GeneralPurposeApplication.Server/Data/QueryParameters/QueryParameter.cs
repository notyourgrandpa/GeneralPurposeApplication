using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralPurposeApplication.Server.Data.QueryParameters
{
    public class QueryParameter
    {
        public int pageIndex = 0;
        public int pageSize = 10;
        public string? sortColumn = null;
        public string? sortOrder = null;
        public string? filterColumn = null;
        public string? filterQuery = null;
    }
}
