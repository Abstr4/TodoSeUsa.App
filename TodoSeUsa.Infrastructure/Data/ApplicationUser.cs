using Microsoft.AspNetCore.Identity;

namespace TodoSeUsa.Infrastructure.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public string RecoveryCodeHash { get; set; } = null!;

    public bool RecoveryCodePageVisited { get; set; } = false;
}