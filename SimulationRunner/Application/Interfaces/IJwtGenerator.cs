using Domain;

namespace Application.Interfaces;

public interface IJwtGenerator
{
    string CreateToken(Domain.User user);
    RefreshToken GenerateRefreshToken();
}