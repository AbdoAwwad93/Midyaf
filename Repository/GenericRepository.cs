using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Midyaf.Data;

namespace Midyaf.Repository;

public class GenericRepository<T>:IGenericRepository<T> where T:class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbset;
    public GenericRepository(AppDbContext context)
    {
        _context =context;
        _dbset = _context.Set<T>();
    }
    
    public async Task AddAsync(T entity)
    {
       await _dbset.AddAsync(entity);
    }

    public async Task RemoveAsync(T entity)
    {
       _dbset.Remove(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        _dbset.Update(entity);
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        var entity = await _dbset.FindAsync(id);
        return entity;
    }

    public async Task<List<T>> GetAllAsync()
    {
        var entities = await _dbset.ToListAsync();
        return entities;
    }

    public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbset.Where(predicate).ToListAsync();
    }

    public IQueryable<T> GetQueryable()
    {
        return _dbset.AsQueryable();
    }
}