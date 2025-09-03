using Microsoft.EntityFrameworkCore;
using Midyaf.Core.Interfaces;
using Midyaf.Infrastructure.Data;
namespace Midyaf.Infrastructure.repository;

public class GenericRepository<T>:IGenericRepository<T> where T:class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbset;
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

    public async Task<T?> GetByIdAsync(string id)
    {
        var entity = await _dbset.FindAsync(id);
        return entity;
    }

    public async Task<List<T>> GetAllAsync()
    {
        var entities = await _dbset.ToListAsync();
        return entities;
    }
}