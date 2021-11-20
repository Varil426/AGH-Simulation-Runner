using Application.Interfaces;
using System.Text.Json.Serialization;

namespace Application.User;

public class User
{
    public User(Domain.User user, IJwtGenerator jwtGenerator, string refreshToken)
    {
        UserName = user.UserName;
        FirstName = user.FirstName;
        LastName = user.LastName;
        RefreshToken = refreshToken;
        Token = jwtGenerator.CreateToken(user);
    }

    public string UserName { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Token { get; set; }

    [JsonIgnore]
    public string RefreshToken { get; set; }
}