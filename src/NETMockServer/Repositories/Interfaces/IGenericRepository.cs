namespace NETMockServer.Repositories.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync(int skip = 0, int take = 100);
    Task<T?> GetByIdAsync(long id);
    Task<T> AddAsync(T entity);
    Task<T?> UpdateAsync(long id, T entity);
    Task<bool> DeleteAsync(long id);
}