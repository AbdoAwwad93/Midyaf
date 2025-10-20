namespace Midyaf.Core.Interfaces;

public interface IGenericRepository<T> where T:class
{
    public Task AddAsync(T entity);
    public Task RemoveAsync(T entity);
    public Task UpdateAsync(T entity);
    public Task<T?> GetByIdAsync(object id);
    public Task<List<T>> GetAllAsync();
}