namespace NETMockServer.DTOs;

public class ProductDto
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<string>? Tags { get; set; }
}