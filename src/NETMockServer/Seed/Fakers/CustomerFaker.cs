using Bogus;
using NETMockServer.Entities;
using NETMockServer.Seed.Interfaces;

namespace NETMockServer.Seed.Fakers;

public class CustomerFaker : IEntityFaker<Customer>
{
    private readonly Faker<Customer> faker = new Faker<Customer>()
        .RuleFor(c => c.FirstName, f => f.Name.FirstName())
        .RuleFor(c => c.LastName, f => f.Name.LastName())
        .RuleFor(c => c.Email, (f, c) => f.Internet.Email(c.FirstName, c.LastName))
        .RuleFor(c => c.RegisteredAt, f => f.Date.Past(3));

    public Customer Generate() => faker.Generate();
}