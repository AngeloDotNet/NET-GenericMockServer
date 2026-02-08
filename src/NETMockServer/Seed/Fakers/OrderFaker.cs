using Bogus;
using NETMockServer.Entities;
using NETMockServer.Seed.Interfaces;

namespace NETMockServer.Seed.Fakers;

public class OrderFaker : IEntityFaker<Order>
{
    private readonly Faker<Order> faker;

    public OrderFaker()
    {
        faker = new Faker<Order>()
            .RuleFor(o => o.CreatedAt, f => f.Date.Recent(60))
            .RuleFor(o => o.Items, f =>
            {
                // Items will be assigned later in seeder where product ids are known.
                return [];
            });
    }

    public Order Generate() => faker.Generate();
}