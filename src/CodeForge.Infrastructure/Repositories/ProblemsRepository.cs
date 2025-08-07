using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Codeforge.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Codeforge.Infrastructure.Repositories;

public class ProblemsRepository(CodeforgeDbContext dbContext) : BaseRepository<Problem>(dbContext), IProblemsRepository {
	private readonly CodeforgeDbContext _dbContext = dbContext;
	private readonly DbSet<Problem> _dbSet = dbContext.Set<Problem>();

	public override async Task<Problem?> GetByIdAsync(int id) {
		var result = await _dbSet
			.AsNoTracking()
			.Include(p => p.Tags)
			.Include(p => p.Submissions)
			.Include(p => p.TestCases)
			.FirstOrDefaultAsync(p => p.Id == id);

		return result;
	}

	public override async Task<(IEnumerable<Problem>?, int count)> GetAllAsync(int pageNumber, int pageSize) {
		var data = await _dbSet
			.AsNoTracking()
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		var count = await _dbSet.CountAsync();
		return (data, count);
	}

	public async Task<Problem?> GetByIdWithTagsAsync(int id, bool asTracking = true) {
		var query = _dbSet
			.Include(p => p.Tags)
			.Where(p => p.Id == id);

		return asTracking
			? await query.FirstOrDefaultAsync()
			: await query.AsNoTracking().FirstOrDefaultAsync();
	}

	public async Task AddTagToProblemAsync(Problem problem, Tag tag) {
		if (_dbContext.Entry(problem).State == EntityState.Detached)
			_dbContext.Attach(problem);

		if (_dbContext.Entry(tag).State == EntityState.Detached)
			_dbContext.Attach(tag);


		if (problem.Tags.All(t => t.Id != tag.Id)) {
			problem.Tags.Add(tag);
			await _dbContext.SaveChangesAsync();
		}
	}

	public async Task RemoveTagFromProblemAsync(Problem problem, int tagId) {
		if (_dbContext.Entry(problem).State == EntityState.Detached)
			_dbContext.Attach(problem);

		var tag = problem.Tags.FirstOrDefault(t => t.Id == tagId);
		if (tag is not null) {
			problem.Tags.Remove(tag);
			await _dbContext.SaveChangesAsync();
		}
	}
}