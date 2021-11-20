using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;

namespace Domain;

public class User : IdentityUser
{
    public User()
    {
        Simulations = new Collection<Simulation>();
        RefreshTokens = new Collection<RefreshToken>();
    }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public virtual ICollection<Simulation> Simulations { get; set; }

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
}
