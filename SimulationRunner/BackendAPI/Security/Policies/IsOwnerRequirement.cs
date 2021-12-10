using Microsoft.AspNetCore.Authorization;
using Persistence;
using System.Security.Claims;

namespace BackendAPI.Security.Policies;

public class IsOwnerRequirement : IAuthorizationRequirement
{
}

public class IsOwnerRequirementHandler : AuthorizationHandler<IsOwnerRequirement>
{
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly DataContext _context;

	public IsOwnerRequirementHandler(IHttpContextAccessor httpContextAccessor, DataContext context)
	{
		_httpContextAccessor = httpContextAccessor;
		_context = context;
	}

	protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsOwnerRequirement requirement)
	{
		var currentUserEmail = _httpContextAccessor.HttpContext?.User?.Claims?.SingleOrDefault(x => x.Type == ClaimTypes.Email)?.Value;
		var simulationId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues.SingleOrDefault(x => x.Key == "simulationId").Value?.ToString() ?? String.Empty);
		var simulation = _context.Simulations.FindAsync(simulationId).Result;
		var owner = simulation?.User;
		if (owner != null && owner.Email == currentUserEmail)
			context.Succeed(requirement);
		return Task.CompletedTask;
	}
}
