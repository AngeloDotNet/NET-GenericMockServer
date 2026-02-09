namespace NETMockServer.Entities;

public class ProductTag
{
    public long ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public long TagId { get; set; }
    public Tag Tag { get; set; } = default!;
}