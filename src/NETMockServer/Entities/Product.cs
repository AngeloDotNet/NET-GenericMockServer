namespace NETMockServer.Entities;

public class Product : EntityBase
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    // Many-to-many via join entity
    public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
}