using Microsoft.EntityFrameworkCore;
using NETMockServer.Data;
using NETMockServer.Entities;
using NETMockServer.Seed.Interfaces;

namespace MockServer.Seed;

public class FakeDataSeeder(IServiceProvider sp, AppDbContext dbContext)
{
    public async Task EnsureSeedAsync<T>(int count = 20) where T : class
    {
        var set = dbContext.Set<T>();
        var any = await set.AsNoTracking().AnyAsync();

        if (any)
        {
            return;
        }

        var fakerType = typeof(IEntityFaker<>).MakeGenericType(typeof(T));
        var faker = sp.GetService(fakerType) ?? throw new InvalidOperationException($"No faker registered for {typeof(T).Name}");

        var generateMethod = fakerType.GetMethod("Generate")!;
        var list = new List<T>();

        for (var i = 0; i < count; i++)
        {
            var item = (T)generateMethod.Invoke(faker, null)!;
            list.Add(item);
        }

        await set.AddRangeAsync(list);
        await dbContext.SaveChangesAsync();

        // Special handling: create relationships after base entities exist
        if (typeof(T) == typeof(Tag))
        {
            // assign tags to random products
            var products = await dbContext.Products!.ToListAsync();
            var tags = await dbContext.Tags!.ToListAsync();
            var rnd = new Random();

            foreach (var p in products)
            {
                var pick = tags.OrderBy(_ => rnd.Next()).Take(rnd.Next(0, 3)).ToList();
                foreach (var t in pick)
                {
                    dbContext.ProductTags!.Add(new ProductTag { ProductId = p.Id, TagId = t.Id });
                }
            }

            await dbContext.SaveChangesAsync();
        }

        if (typeof(T) == typeof(Order))
        {
            var customers = await dbContext.Customers!.ToListAsync();
            var products = await dbContext.Products!.ToListAsync();
            var rnd = new Random();

            // generate order items across created orders
            var orders = await dbContext.Orders!.Where(o => o.Items.Count == 0).ToListAsync();

            foreach (var o in orders)
            {
                o.CustomerId = customers[rnd.Next(customers.Count)].Id;
                var itemsCount = rnd.Next(1, 4);

                for (var j = 0; j < itemsCount; j++)
                {
                    var prod = products[rnd.Next(products.Count)];
                    o.Items.Add(new OrderItem
                    {
                        ProductId = prod.Id,
                        Quantity = rnd.Next(1, 5),
                        UnitPrice = prod.Price
                    });
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}