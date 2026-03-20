using Asisya.Products.Domain.Common;

namespace Asisya.Products.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? ImageUrl { get; private set; }
    public ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { }

    public Category(string name, string? imageUrl = null)
    {
        Name = name;
        ImageUrl = imageUrl;
    }

    public void Update(string name, string? imageUrl = null)
    {
        Name = name;
        ImageUrl = imageUrl;
        SetUpdatedAt();
    }
}
