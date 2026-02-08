using System.Reflection;
using Bogus;
using NETMockServer.Seed.Interfaces;

namespace NETMockServer.Seed.Fakers;

public class DefaultEntityFaker<T> : IEntityFaker<T> where T : class, new()
{
    private readonly Faker faker = new();

    public T Generate()
    {
        var inst = new T();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite && p.Name != "Id");

        foreach (var p in props)
        {
            var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

            if (t == typeof(string))
            {
                p.SetValue(inst, faker.Lorem.Word());
            }
            else if (t == typeof(int) || t == typeof(long))
            {
                p.SetValue(inst, Convert.ChangeType(faker.Random.Int(1, 9999), t));
            }
            else if (t == typeof(decimal) || t == typeof(double) || t == typeof(float))
            {
                p.SetValue(inst, Convert.ChangeType(faker.Random.Decimal(1, 9999), t));
            }
            else if (t == typeof(bool))
            {
                p.SetValue(inst, faker.Random.Bool());
            }
            else if (t == typeof(DateTime))
            {
                p.SetValue(inst, faker.Date.Recent(365));
            }
        }

        return inst;
    }
}