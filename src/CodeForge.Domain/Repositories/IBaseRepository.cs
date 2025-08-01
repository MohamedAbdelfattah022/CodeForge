using CodeForge.Domain.Entities;

namespace CodeForge.Domain.Repositories;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity {
	Task<TEntity?> GetByIdAsync(int id);
	Task<(IEnumerable<TEntity>?, int count)> GetAllAsync(int pageNumber, int pageSize);
	Task<int> CreateAsync(TEntity entity);
	Task UpdateAsync(TEntity entity);
	Task DeleteAsync(TEntity entity);
	Task<bool> ExistsAsync(int id);
}