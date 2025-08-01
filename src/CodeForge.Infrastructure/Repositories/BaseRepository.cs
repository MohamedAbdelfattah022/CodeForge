using CodeForge.Domain.Entities;
using CodeForge.Domain.Repositories;
using CodeForge.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CodeForge.Infrastructure.Repositories;

public class BaseRepository<TEntity>(CodeForgeDbContext dbContext) : IBaseRepository<TEntity> where TEntity : BaseEntity {
	private readonly DbSet<TEntity> _dbSet = dbContext.Set<TEntity>();

	public virtual async Task<TEntity?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);


	public virtual async Task<(IEnumerable<TEntity>?, int count)> GetAllAsync(int pageNumber, int pageSize) {
		var data = await _dbSet
			.AsNoTracking()
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		var count = await _dbSet.CountAsync();

		return (data, count);
	}

	public async Task<int> CreateAsync(TEntity entity) {
		await _dbSet.AddAsync(entity);
		await dbContext.SaveChangesAsync();
		return entity.Id;
	}

	public async Task UpdateAsync(TEntity entity) {
		_dbSet.Update(entity);
		await dbContext.SaveChangesAsync();
	}

	public async Task DeleteAsync(TEntity entity) {
		_dbSet.Remove(entity);
		await dbContext.SaveChangesAsync();
	}

	public async Task<bool> ExistsAsync(int id) {
		return await _dbSet.AnyAsync(e => e.Id == id);
	}
}