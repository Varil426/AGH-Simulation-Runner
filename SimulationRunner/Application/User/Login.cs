using Application.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Application.User;

public class Login
{
    public class Query : IRequest<User>
    {
        public string? Email { get; set; }

        public string? Password { get; set; }
    }

    public class QueryValidator : AbstractValidator<Query>
    {
        public QueryValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class Handler : IRequestHandler<Query, User>
    {
        private readonly UserManager<Domain.User> _userManager;
        private readonly SignInManager<Domain.User> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IMapper _mapper;

        public Handler(UserManager<Domain.User> userManager, SignInManager<Domain.User> signInManager, IJwtGenerator jwtGenerator, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
            _mapper = mapper;
        }

        public async Task<User> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                throw new RestException(System.Net.HttpStatusCode.NotFound, new { Email = "Email not found." });
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (result.Succeeded)
            {
                var refreshToken = _jwtGenerator.GenerateRefreshToken();
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
                return _mapper.Map<Domain.User, User>(user, options => options.ConstructServicesUsing(x => new User(user, _jwtGenerator, refreshToken.Token)));//new User(user, _jwtGenerator, refreshToken.Token);
            }
        
            throw new RestException(System.Net.HttpStatusCode.Unauthorized);
        }
    }
}