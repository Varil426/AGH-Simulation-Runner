using Application.Interfaces;
using SimulationHandler;

namespace Application.Simulation;
public class Create
{
    public class Command : IRequest<SimulationDto>
    {
        public string Name { get; set; } = null!;

        public IFormFile File { get; set; } = null!;

        public string? Version { get; set; }
    }

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.File).NotEmpty();
            //RuleFor(x => x.FileType).NotEmpty().IsInEnum().Must(x => x != Domain.Simulation.AllowedFileTypesEnum.Uknown);
        }
    }

    public class Handler : IRequestHandler<Command, SimulationDto>
    {
        private readonly DataContext _dataContext;
        private readonly IUserAccessor _userAccessor;
        private readonly IMapper _mapper;
        private readonly ISimulationHandler _simulationHandler;

        public Handler(DataContext dataContext, IUserAccessor userAccessor, IMapper mapper, ISimulationHandler simulationHandler)
        {
            _dataContext = dataContext;
            _userAccessor = userAccessor;
            _mapper = mapper;
            _simulationHandler = simulationHandler;
        }

        public async Task<SimulationDto> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await _dataContext.Users.SingleOrDefaultAsync(user => user.Email == _userAccessor.GetCurrentUserEmailAddress())
                ?? throw new RestException(System.Net.HttpStatusCode.InternalServerError, new { User = "Not found."});

            var basePath = Path.Combine(Directory.GetCurrentDirectory() + @"\Files\");
            /*if (!Directory.Exists(basePath))
                Directory.CreateDirectory(basePath);
            var filePath = Path.Combine(basePath, request.File.FileName);*/
            var fileType = Path.GetExtension(request.File.FileName).ToLowerInvariant() switch
            {
                ".zip" => Domain.Simulation.AllowedFileTypesEnum.ZIP,
                ".dll" => Domain.Simulation.AllowedFileTypesEnum.DLL,
                _ => Domain.Simulation.AllowedFileTypesEnum.Uknown
            };

            /*if (!File.Exists(filePath))
            {
                using var stream = new FileStream(filePath, FileMode.Create);
                await request.File.CopyToAsync(stream);
            }*/

            var validationResults = await ValidateSimulation(request.File, fileType);
            if (validationResults.Any())
                throw new RestException(System.Net.HttpStatusCode.BadRequest, new { Simulation = validationResults });

            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream);

            // TODO Validate file - extension and content

            var simulation = new Domain.Simulation(request.Name, memoryStream.ToArray(), fileType);
            user.Simulations.Add(simulation);
            if (await _dataContext.SaveChangesAsync() > 0)
                return _mapper.Map<SimulationDto>(simulation);
            throw new Exception("Problem saving changes.");
        }

        private async Task<List<string>> ValidateSimulation(IFormFile simulationFile, Domain.Simulation.AllowedFileTypesEnum fileType)
        {
            // Save to temporary file
            var tempFilePath = Path.GetTempFileName();
            using var fileStream = new FileStream(tempFilePath, FileMode.Create);
            await simulationFile.CopyToAsync(fileStream);
            fileStream.Close();
            
            switch (fileType)
            {
                case Domain.Simulation.AllowedFileTypesEnum.DLL:
                    _simulationHandler.CheckSimulationAssembly(tempFilePath, out var errors);
                    return errors;
                case Domain.Simulation.AllowedFileTypesEnum.ZIP: // TODO
                default:
                    throw new RestException(System.Net.HttpStatusCode.BadRequest, new { Simulation = "File type not allowed" });
            }
        }
    }
}