using Application.Errors;
using Application.Interfaces;
using System.Security.Claims;

namespace BackendAPI.Security;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetCurrentUsername() => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, new { User = "User claim not found." });

    public string GetCurrentUserEmailAddress() =>_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value
        ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, new { Email = "Email claim not found." });
}