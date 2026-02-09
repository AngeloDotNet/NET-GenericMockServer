namespace NETMockServer.Entities;

public class Customer : EntityBase
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime RegisteredAt { get; set; }

    // 1:N relation
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}