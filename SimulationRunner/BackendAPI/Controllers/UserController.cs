using Application.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendAPI.Controllers;

public class UserController : BaseController
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register(Register.Command command)
    {
        await Mediator.Send(command);
        return Ok("Registered successfully.");
    }
}