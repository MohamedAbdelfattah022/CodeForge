using CodeForge.Application.Dtos;
using CodeForge.Domain.Entities;
using CodeForge.Domain.Repositories;
using CodeForge.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace CodeForge.Infrastructure.Repositories;

public class ProblemsRepository(CodeForgeDbContext dbContext) : IProblemsRepository {
	public async Task<Problem?> GetByIdAsync(int id) {
		var result = await dbContext.Problems
			.AsNoTracking()
			.Include(p => p.Tags)
			.Include(p => p.Submissions)
			.Include(p => p.TestCases)
			.FirstOrDefaultAsync(p => p.Id == id);

		return result;
	}

	public async Task<(IEnumerable<Problem>?, int count)> GetAllAsync(int pageNumber, int pageSize) {
		var data = await dbContext.Problems
			.AsNoTracking()
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		var count = await dbContext.Problems.CountAsync();
		return (data, count);
	}

	public async Task<int> CreateAsync(Problem problem) {
		await dbContext.Problems.AddAsync(problem);
		await dbContext.SaveChangesAsync();
		return problem.Id;
	}

	public async Task UpdateAsync(Problem problem) {
		dbContext.Problems.Update(problem);
		await dbContext.SaveChangesAsync();
	}

	public async Task DeleteAsync(Problem problem) {
		dbContext.Problems.Remove(problem);
		await dbContext.SaveChangesAsync();
	}
}