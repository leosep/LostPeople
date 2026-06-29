using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using LostPeople.Domain.Interfaces;

namespace LostPeople.Infrastructure.Persistence;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly LostPeopleDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(LostPeopleDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default) =>
        await _dbSet.FindAsync(new object[] { id }, ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default) =>
        await _dbSet.ToListAsync(ct);

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await _dbSet.Where(predicate).ToListAsync(ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        return entity;
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
    {
        if (predicate == null) return await _dbSet.CountAsync(ct);
        return await _dbSet.CountAsync(predicate, ct);
    }

    public IQueryable<T> Query() => _dbSet.AsQueryable();
}
