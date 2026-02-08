using Microsoft.EntityFrameworkCore;
using NETMockServer.Data;
using NETMockServer.Repositories.Interfaces;

namespace NETMockServer.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly AppDbContext dbContext;
    private readonly DbSet<T> set;

    public GenericRepository(AppDbContext db)
    {
        this.dbContext = db;
        set = this.dbContext.Set<T>();
    }

    public async Task<T> AddAsync(T entity)
    {
        var entry = await set.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return entry.Entity;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var found = await FindByIdAsync(id);
        if (found is null)
        {
            return false;
        }

        set.Remove(found);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<T>> GetAllAsync(int skip = 0, int take = 100)
        => await set.AsNoTracking().Skip(skip).Take(take).ToListAsync();

    public async Task<T?> GetByIdAsync(long id)
        => await FindByIdAsync(id);

    public async Task<T?> UpdateAsync(long id, T entity)
    {
        var found = await FindByIdAsync(id);

        if (found is null)
        {
            return null;
        }

        var props = typeof(T).GetProperties()
            .Where(p => p.CanWrite && !string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase));

        foreach (var p in props)
        {
            var val = p.GetValue(entity);
            p.SetValue(found, val);
        }

        await dbContext.SaveChangesAsync();
        return found;
    }

    private async Task<T?> FindByIdAsync(long id)
        => await set.FindAsync(id) as T;
}