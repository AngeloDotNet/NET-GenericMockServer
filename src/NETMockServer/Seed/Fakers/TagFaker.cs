using Bogus;
using NETMockServer.Entities;
using NETMockServer.Seed.Interfaces;

namespace NETMockServer.Seed.Fakers;

public class TagFaker : IEntityFaker<Tag>
{
    private readonly Faker<Tag> faker = new Faker<Tag>()
        .RuleFor(t => t.Name, f => f.Lorem.Word());

    public Tag Generate() => faker.Generate();
}