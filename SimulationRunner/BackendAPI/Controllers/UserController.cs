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

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<User>> Login(Login.Query query)
    {
        var user = await Mediator.Send(query);
        SetTokenCookie(user.RefreshToken);
        return user;
    }

    [HttpGet]
    public async Task<ActionResult<User>> GetCurrentUser()
    {
        var user = await Mediator.Send(new CurrentUser.Query());
        SetTokenCookie(user.RefreshToken);
        return user;
    }

    private void SetTokenCookie(string refreshToken)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };
        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}