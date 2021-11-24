using Application.Extensions;
using Application.Interfaces;

namespace Application.Simulation;

public class List
{
    public class Query : IRequest<SimulationCollection> { }

    public class SimulationCollection
    {
        public List<SimulationDto>? Simulations { get; init; }
        public int? SimulationCount => Simulations?.Count;
    }

    public class Handler : IRequestHandler<Query, SimulationCollection>
    {
        private readonly UserManager<Domain.User> _userManager;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;

        public Handler(UserManager<Domain.User> userManager, IUserAccessor userAccessor, IMapper mapper)
        {
            _userManager = userManager;
            _userAccessor = userAccessor;
            _mapper = mapper;
        }

        public async Task<SimulationCollection> Handle(Query request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(_userAccessor.GetCurrentUserEmailAddress());

            var simulations = await user.Simulations.OrderBy(x => x.Name).AsQueryable().ToListAsyncSafe();

            return new SimulationCollection
            {
                Simulations = _mapper.Map<List<Domain.Simulation>, List<SimulationDto>>(simulations)
            };
        }
    }
}