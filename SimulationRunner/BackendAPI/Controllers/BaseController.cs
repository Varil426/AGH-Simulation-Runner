using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
/*[Authorize]*/
public abstract class BaseController : ControllerBase
{
	private IMediator? _mediator;
	protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}