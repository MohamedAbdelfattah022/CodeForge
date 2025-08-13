using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Codeforge.Infrastructure.Contexts;

namespace Codeforge.Infrastructure.Repositories;

public class SubmissionsRepository(CodeforgeDbContext dbContext) : BaseRepository<Submission>(dbContext), ISubmissionsRepository { }