using Application.Interfaces;
using System.Text.Json.Serialization;

namespace Application.User;

public class UserDto
{
    public UserDto(Domain.User user, IJwtGenerator jwtGenerator, string refreshToken)
    {
        Token = jwtGenerator.CreateToken(user);
        RefreshToken = refreshToken;
    }

    public string UserName { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Token { get; set; }

    [JsonIgnore]
    public string RefreshToken { get; set; }
}