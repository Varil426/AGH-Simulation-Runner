using Application.Interfaces;
using System.Text.Json;

namespace Application.Simulation;

public class Run
{
    public class Command : IRequest
    {
        public Guid SimulationId { get; set; }
        public List<Dictionary<string, JsonElement>> Parameters { get; set; } = null!;
    }

    public class Handler : IRequestHandler<Command>
    {
        private readonly DataContext _dataContext;
        private readonly IUserAccessor _userAccessor;
        private readonly IDockerContainerManager _dockerContainerManager;

        public Handler(DataContext dataContext, IUserAccessor userAccessor, IDockerContainerManager dockerContainerManager)
        {
            _dataContext = dataContext;
            _userAccessor = userAccessor;
            _dockerContainerManager = dockerContainerManager;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            /*var user = await _dataContext.Users.FirstOrDefaultAsync(user => user.Email == _userAccessor.GetCurrentUserEmailAddress()) 
                ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, new { User = "Not found"});*/
            var simulation = await _dataContext.Simulations.FirstOrDefaultAsync(simulation => simulation.Id == request.SimulationId)
                ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, new { Simulation = "Not found" });

            foreach (var parameters in request.Parameters)
            {
                _dockerContainerManager.RunSimulation(simulation, parameters);
            }
            
            return Unit.Value;
        }
    }
}