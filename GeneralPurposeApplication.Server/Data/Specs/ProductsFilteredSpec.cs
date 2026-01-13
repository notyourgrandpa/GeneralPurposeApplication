using GeneralPurposeApplication.Server.Data.Models;
using GeneralPurposeApplication.Server.Data.QueryParameters;
using System.Linq.Expressions;

namespace GeneralPurposeApplication.Server.Data.Specs
{
    public class ProductsFilteredSpec : Specification<Product>
    {
        public ProductsFilteredSpec(ProductQueryParameter param)
        {
            Criteria = p =>
                (param.categoryId.HasValue && p.CategoryId == param.categoryId) &&
                (string.IsNullOrEmpty(param.filterQuery) || p.Name.Contains(param.filterQuery));

            Includes.Add(p => p.Category!);


            if(!string.IsNullOrWhiteSpace(param.sortColumn) && ProductSortMap.Map.TryGetValue(param.sortColumn, out var sortExpression))
            {
                if(param.sortOrder == "asc")
                    OrderBy = sortExpression;
                else if(param.sortOrder == "desc")
                    OrderByDescending = sortExpression;
            }

            Skip = param.pageIndex * param.pageSize;
            Take = param.pageSize;
            IsPagingEnabled = true;
        }
    }
    
    public static class ProductSortMap
    {
        public static readonly IReadOnlyDictionary<string, Expression<Func<Product, object>>> Map = new Dictionary<string, Expression<Func<Product, object>>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Name"] = p => p.Name,
            ["CategoryName"] = p => p.Category!.Name,
            ["Category"] = p => p.CategoryId,
            ["CostPrice"] = p => p.CostPrice,
            ["SellingPrice"] = p => p.SellingPrice,
            ["Stock"] = p => p.Stock,
            ["DateAdded"] = p => p.DateAdded,
            ["DateUpdated"] = p => p.LastUpdated,
        };
    }
}
