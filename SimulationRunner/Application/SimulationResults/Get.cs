namespace Application.SimulationResults;

public class Get
{
    public class Query : IRequest<SimulationResultsDto>
    {
        public Guid SimulationId;
        public int AttemptNumber;
    }

    public class Handler : IRequestHandler<Query, SimulationResultsDto>
    {
        private readonly DataContext _dataContext;

        public Handler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<SimulationResultsDto> Handle(Query request, CancellationToken cancellationToken)
        {
            var simulation = await _dataContext.Simulations.FindAsync(request.SimulationId) ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, new { Simulation = "Not found." });
            var runAttempt = simulation.SimulationRunAttempts.FirstOrDefault(x => x.AttemptNumer == request.AttemptNumber) ?? throw new RestException(System.Net.HttpStatusCode.BadRequest, new { RunAttempt = "Not found." });
            return new SimulationResultsDto(runAttempt);
        }
    }
}