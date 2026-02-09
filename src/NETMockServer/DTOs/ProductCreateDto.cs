namespace NETMockServer.DTOs;

public class ProductCreateDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public ICollection<long>? TagIds { get; set; }
}