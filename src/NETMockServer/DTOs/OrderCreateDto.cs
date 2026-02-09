namespace NETMockServer.DTOs;

public class OrderCreateDto
{
    public long CustomerId { get; set; }
    public ICollection<OrderItemCreateDto> Items { get; set; } = new List<OrderItemCreateDto>();
}

public class OrderItemCreateDto
{
    public long ProductId { get; set; }
    public int Quantity { get; set; }
}