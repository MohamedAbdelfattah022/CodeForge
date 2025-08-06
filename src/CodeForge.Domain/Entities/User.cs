using Microsoft.AspNetCore.Identity;

namespace Codeforge.Domain.Entities;

public sealed class User : IdentityUser {
	public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}