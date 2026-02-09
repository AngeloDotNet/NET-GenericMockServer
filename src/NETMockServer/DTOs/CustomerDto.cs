namespace NETMockServer.DTOs;

public class CustomerDto
{
    public long Id { get; set; }
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public DateTime RegisteredAt { get; set; }
}