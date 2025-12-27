using GeneralPurposeApplication.Server.Data.Models;

namespace GeneralPurposeApplication.Server.Data.Specs
{
    public class ProductsByCategorySpec : Specification<Product>
    {
        public ProductsByCategorySpec(
            int categoryId,
            string? search,
            int pageIndex,
            int pageSize)
        {
            Criteria = p =>
                p.CategoryId == categoryId &&
                (string.IsNullOrEmpty(search) || p.Name.Contains(search));

            Includes.Add(p => p.Category!);

            OrderBy = p => p.Name;

            Skip = pageIndex * pageSize;
            Take = pageSize;
            IsPagingEnabled = true;
        }
    }

}
