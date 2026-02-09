using AutoMapper;
using FluentValidation;
using NETMockServer.Repositories.Interfaces;

namespace NETMockServer.Extensions;

// Minimal helper - registers generic endpoints per entity type
public static class EndpointExtensions
{
    //public static void MapEntityEndpoints<T>(this WebApplication app, string routeBase) where T : class
    //{
    //    var basePath = $"/api/{routeBase}";
    //    var mapGroup = app.MapGroup(basePath)
    //        .WithTags(routeBase)
    //        .WithDescription($"Operations for {routeBase}");

    //    mapGroup.MapGet(string.Empty, async (IGenericRepository<T> repository, int? skip, int? take) =>
    //    {
    //        var items = await repository.GetAllAsync(skip ?? 0, take ?? 100);

    //        return Results.Ok(items);
    //    });

    //    mapGroup.MapGet($"/{{id}}", async (IGenericRepository<T> repository, Guid id) =>
    //    {
    //        var item = await repository.GetByIdAsync(id);

    //        return item is null ? Results.NotFound() : Results.Ok(item);
    //    });

    //    mapGroup.MapPost(string.Empty, async (IGenericRepository<T> repository, T dto) =>
    //    {
    //        var created = await repository.AddAsync(dto);

    //        return Results.Created($"{basePath}/{GetId(created)}", created);
    //    });

    //    mapGroup.MapPut($"/{{id}}", async (IGenericRepository<T> repository, Guid id, T dto) =>
    //    {
    //        var updated = await repository.UpdateAsync(id, dto);

    //        return updated is null ? Results.NotFound() : Results.Ok(updated);
    //    });

    //    mapGroup.MapDelete($"/{{id}}", async (IGenericRepository<T> repository, Guid id) =>
    //    {
    //        var deleted = await repository.DeleteAsync(id);

    //        return deleted ? Results.NoContent() : Results.NotFound();
    //    });
    //}

    //// Try to read a property named "Id" or "ID" or "id" as Guid
    //static object? GetId<T>(T entity)
    //{
    //    var prop = typeof(T).GetProperty("Id") ?? typeof(T).GetProperty("ID") ?? typeof(T).GetProperty("id");
    //    return prop?.GetValue(entity);
    //}

    // Simple generic endpoints for Product and Customer using DTOs, AutoMapper & FluentValidation
    public static void MapEntityEndpoints<T, TDto, TCreateDto>(RouteGroupBuilder group, string routeBase)
        where T : class
        where TDto : class
        where TCreateDto : class
    {
        var basePath = group.MapGroup($"/{routeBase}");

        basePath.MapGet("/", async (IGenericRepository<T> repo, IMapper mapper, int? skip, int? take) =>
        {
            var items = await repo.GetAllAsync(skip ?? 0, take ?? 100);

            return Results.Ok(mapper.Map<IEnumerable<TDto>>(items));
        });

        basePath.MapGet("/{id:long}", async (IGenericRepository<T> repo, IMapper mapper, long id) =>
        {
            var item = await repo.GetByIdAsync(id);

            return item is null ? Results.NotFound() : Results.Ok(mapper.Map<TDto>(item));
        });

        basePath.MapPost("/", async (IGenericRepository<T> repo, IMapper mapper, IValidator<TCreateDto>? validator, TCreateDto dto) =>
        {
            if (validator is not null)
            {
                var validation = await validator.ValidateAsync(dto);
                if (!validation.IsValid)
                {
                    return Results.ValidationProblem(validation.ToDictionary());
                }
            }

            var entity = mapper.Map<T>(dto);
            var created = await repo.AddAsync(entity);
            var createdDto = mapper.Map<TDto>(created);

            return Results.Created($"/api/v1/{routeBase}/{Helpers.GetId(created)}", createdDto);
        });

        basePath.MapPut("/{id:long}", async (IGenericRepository<T> repo, IMapper mapper, IValidator<TCreateDto>? validator, long id, TCreateDto dto) =>
        {
            if (validator is not null)
            {
                var validation = await validator.ValidateAsync(dto);
                if (!validation.IsValid)
                {
                    return Results.ValidationProblem(validation.ToDictionary());
                }
            }

            var entity = mapper.Map<T>(dto);
            var updated = await repo.UpdateAsync(id, entity);

            return updated is null ? Results.NotFound() : Results.Ok(mapper.Map<TDto>(updated));
        });

        basePath.MapDelete("/{id:long}", async (IGenericRepository<T> repo, long id) =>
        {
            var deleted = await repo.DeleteAsync(id);

            return deleted ? Results.NoContent() : Results.NotFound();
        });
    }
}