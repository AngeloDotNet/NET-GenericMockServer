namespace NETMockServer.Entities;

public class Tag : EntityBase
{
    public string Name { get; set; } = default!;
    public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();
}