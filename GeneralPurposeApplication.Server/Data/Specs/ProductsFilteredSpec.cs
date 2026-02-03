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
                (!param.CategoryId.HasValue || p.CategoryId == param.CategoryId) &&
                (string.IsNullOrEmpty(param.FilterQuery) || p.Name.Contains(param.FilterQuery)) &&
                (!param.IsActive.HasValue || p.IsActive == param.IsActive);

            Includes.Add(p => p.Category!);


            if(!string.IsNullOrWhiteSpace(param.SortColumn) && ProductSortMap.Map.TryGetValue(param.SortColumn, out var sortExpression))
            {
                if(param.SortOrder == "asc")
                    OrderBy = sortExpression;
                else if(param.SortOrder == "desc")
                    OrderByDescending = sortExpression;
            }

            Skip = param.PageIndex * param.PageSize;
            Take = param.PageSize;
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
