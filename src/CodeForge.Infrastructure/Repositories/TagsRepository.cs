using Codeforge.Domain.Entities;
using Codeforge.Domain.Repositories;
using Codeforge.Infrastructure.Contexts;

namespace Codeforge.Infrastructure.Repositories;

public class TagsRepository(CodeforgeDbContext dbContext) : BaseRepository<Tag>(dbContext), ITagsRepository { }