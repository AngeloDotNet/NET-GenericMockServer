using System.Text.Json.Serialization;
using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MockServer.Seed;
using NETMockServer.Data;
using NETMockServer.DTOs;
using NETMockServer.Entities;
using NETMockServer.Extensions;
using NETMockServer.Mapping;
using NETMockServer.Middleware;
using NETMockServer.Repositories;
using NETMockServer.Repositories.Interfaces;
using NETMockServer.Seed.Fakers;
using NETMockServer.Seed.Interfaces;
using NETMockServer.Validators;

namespace NETMockServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddHttpContextAccessor();
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

        // Repositories
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        // AutoMapper
        builder.Services.AddAutoMapper(typeof(MappingProfile));

        // FluentValidation: register validators from this assembly
        builder.Services.AddValidatorsFromAssemblyContaining<ProductCreateDtoValidator>();

        // Fakers & Seeder
        builder.Services.AddTransient(typeof(IEntityFaker<>), typeof(DefaultEntityFaker<>));
        builder.Services.AddTransient<IEntityFaker<Product>, ProductFaker>();
        builder.Services.AddTransient<IEntityFaker<Customer>, CustomerFaker>();
        builder.Services.AddTransient<IEntityFaker<Tag>, TagFaker>();
        builder.Services.AddTransient<IEntityFaker<Order>, OrderFaker>();
        builder.Services.AddScoped<FakeDataSeeder>();

        var app = builder.Build();
        app.UseHttpsRedirection();

        // Middleware - logging request/response
        app.UseMiddleware<RequestResponseLoggingMiddleware>();

        var appName = app.Environment.ApplicationName;
        var swaggerJson = "/openapi/v1.json";
        var swaggerTitle = string.IsNullOrWhiteSpace(appName) ? "API v1" : $"{appName} v1";

        app.MapOpenApi();
        app.MapSwaggerUI("", options => options.SwaggerEndpoint(swaggerJson, swaggerTitle));

        // Ensure DB + seed
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure database: if it already exists, reset for demo purposes; then apply migrations (creates DB when missing).
            if (await db.Database.CanConnectAsync())
            {
                // For demo purposes, always reset the database when it exists.
                await db.Database.EnsureDeletedAsync();
            }

            // Apply migrations (will create DB/schema when missing)
            await db.Database.MigrateAsync();

            var seeder = scope.ServiceProvider.GetRequiredService<FakeDataSeeder>();
            await seeder.EnsureSeedAsync<Product>(30);
            await seeder.EnsureSeedAsync<Customer>(10);
            await seeder.EnsureSeedAsync<Tag>(8);
            //await seeder.EnsureSeedAsync<Order>(25); // Orders are not seeded by default
        }

        app.UseRouting();

        // Versioned route groups (URL-based versioning)
        var v1 = app.MapGroup("/api/v1");

        // Map product/customer endpoints (DTO-based)
        EndpointExtensions.MapEntityEndpoints<Product, ProductDto, ProductCreateDto>(v1, "products");
        EndpointExtensions.MapEntityEndpoints<Customer, CustomerDto, CustomerCreateDto>(v1, "customers");

        #region "Map endpoints for Order entities"

        // Orders need to include related data and return DTO with items and customer
        var orders = v1.MapGroup("/orders");

        orders.MapGet("/", async (AppDbContext db) =>
        {
            var list = await db.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .AsNoTracking()
                .ToListAsync();

            return Results.Ok(list);
        });

        orders.MapGet("/{id:long}", async (AppDbContext db, long id) =>
        {
            var order = await db.Orders
                .Include(o => o.Items)
                .Include(o => o.Customer)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            return order is null ? Results.NotFound() : Results.Ok(order);
        });

        orders.MapPost("/", async (AppDbContext db, IMapper mapper, IValidator<OrderCreateDto> validator, OrderCreateDto dto) =>
        {
            var validation = await validator.ValidateAsync(dto);

            if (!validation.IsValid)
            {
                return Results.ValidationProblem(validation.ToDictionary());
            }

            var order = mapper.Map<Order>(dto);

            // Ensure prices are copied from products if needed
            foreach (var it in order.Items)
            {
                var prod = await db.Products.FindAsync(it.ProductId);

                if (prod != null)
                {
                    it.UnitPrice = prod.Price;
                }
            }

            db.Orders.Add(order);
            await db.SaveChangesAsync();

            return Results.Created($"/api/v1/orders/{order.Id}", mapper.Map<OrderDto>(order));
        });

        orders.MapDelete("/{id:long}", async (AppDbContext db, long id) =>
        {
            var found = await db.Orders.FindAsync(id);
            if (found is null)
            {
                return Results.NotFound();
            }

            db.Orders.Remove(found);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });

        #endregion

        app.Run();
    }
}