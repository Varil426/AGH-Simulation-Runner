using System.Collections.ObjectModel;
using System.Net;

namespace Application.SimulationResults;

public class List
{
    public class Query : IRequest<ResultsCollection>
    {
        public Guid SimulationId { get; init; }
    }

    public class ResultsCollection : Collection<SimulationResultsDto> { }

    public class Handler : IRequestHandler<Query, ResultsCollection>
    {
        private readonly DataContext _dataContext;

        public Handler(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ResultsCollection> Handle(Query request, CancellationToken cancellationToken)
        {
            var simulation = await _dataContext.Simulations.FindAsync(request.SimulationId) ?? throw new RestException(HttpStatusCode.BadRequest, new {Simulation = "Not found."});

            var results = new ResultsCollection();

            foreach (var runAttempt in simulation.SimulationRunAttempts)
                results.Add(new SimulationResultsDto(runAttempt));


            return results;
        }
    }
}