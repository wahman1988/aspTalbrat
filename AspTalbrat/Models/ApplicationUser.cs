using Microsoft.AspNetCore.Identity;

namespace AspTalbrat.Models
{
    public class ApplicationUser: IdentityUser
    {
		public string? Name { get; set; }
	}
}
