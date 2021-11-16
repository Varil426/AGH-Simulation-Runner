using Microsoft.AspNetCore.Identity;
using System.Collections.ObjectModel;

namespace Domain;

public class User : IdentityUser
{
    public User()
    {
        Simulations = new Collection<Simulation>();
    }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public virtual ICollection<Simulation> Simulations { get; set; }
}
