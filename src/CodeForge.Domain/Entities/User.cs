using Microsoft.AspNetCore.Identity;

namespace CodeForge.Domain.Entities;

public class User : IdentityUser {
	public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}