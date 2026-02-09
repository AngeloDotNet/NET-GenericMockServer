namespace NETMockServer.Entities;

public class Order : EntityBase
{
    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> Items { get; set; } = [];
}