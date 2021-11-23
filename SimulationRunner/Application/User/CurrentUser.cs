using Application.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Application.User;

public class CurrentUser
{
    public class Query : IRequest<UserDto> { }

    public class Handler : IRequestHandler<Query, UserDto>
    {
        private readonly UserManager<Domain.User> _userManager;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;

        public Handler(UserManager<Domain.User> userManager, IJwtGenerator jwtGenerator, IUserAccessor userAccessor, IMapper mapper)
        {
            _userManager = userManager;
            _jwtGenerator = jwtGenerator;
            _userAccessor = userAccessor;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(_userAccessor.GetCurrentUsername());
            var refreshToken = _jwtGenerator.GenerateRefreshToken();
            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);
            return _mapper.Map<Domain.User, UserDto>(user, options => options.ConstructServicesUsing(x => new UserDto(user, _jwtGenerator, refreshToken.Token)));
        }
    }
}