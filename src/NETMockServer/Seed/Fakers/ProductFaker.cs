using Bogus;
using NETMockServer.Entities;
using NETMockServer.Seed.Interfaces;

namespace NETMockServer.Seed.Fakers;

public class ProductFaker : IEntityFaker<Product>
{
    private readonly Faker<Product> faker = new Faker<Product>()
        .RuleFor(p => p.Name, f => f.Commerce.ProductName())
        .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
        .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
        .RuleFor(p => p.CreatedAt, f => f.Date.Past(2));

    public Product Generate() => faker.Generate();
}