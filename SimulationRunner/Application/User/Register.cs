using Application.Errors;
using Application.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Application.User;
public class Register
{
    public class Command : IRequest
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty();
            RuleFor(x => x.Email).EmailAddress();
            RuleFor(x => x.Password).Password();
        }
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly DataContext _dataContext;

        private readonly UserManager<Domain.User> _userManager;

        public Handler(DataContext dataContext, UserManager<Domain.User> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            if (await _dataContext.Users.AnyAsync(x => x.UserName == request.UserName))
                throw new RestException(HttpStatusCode.BadRequest, new { UserName = "UserName already exists." });

            if (await _dataContext.Users.AnyAsync(x => x.Email == request.Email))
                throw new RestException(HttpStatusCode.BadRequest, new { Email = "Email already exists." });

            var newUser = new Domain.User
            {
                UserName = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);
            if (!result.Succeeded)
                throw new RestException(HttpStatusCode.InternalServerError, new { Message = "Couldn't create a user."});

            return Unit.Value;
        }
    }
}