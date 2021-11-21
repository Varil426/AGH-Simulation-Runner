using AutoMapper;

namespace Application.MappingProfiles;
public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Domain.User, User.User>().ConstructUsingServiceLocator();
    }
}