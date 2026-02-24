using GeneralPurposeApplication.Domain.Products;

namespace GeneralPurposeApplication.Domain.Categories
{
    public class Category
    {
        public int Id { get; set; }

        public required string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
