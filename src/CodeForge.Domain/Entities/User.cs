using Microsoft.AspNetCore.Identity;

namespace CodeForge.Domain.Entities;

public sealed class User : IdentityUser {
	public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}