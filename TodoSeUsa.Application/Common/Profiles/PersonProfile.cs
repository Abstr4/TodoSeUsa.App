namespace TodoSeUsa.Application.Profiles;

public class PersonProfile : Profile
{
    public PersonProfile()
    {
        CreateMap<PersonalInformationDto, Person>()
            .ConstructUsing(dto => Person.Create(
                dto.FirstName,
                dto.LastName,
                dto.EmailAddress,
                dto.PhoneNumber,
                dto.Address
            ));
        CreateMap<Person, PersonalInformationDto>();

        CreateMap<UpdatePersonalInformationDto, Person>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
            .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember, context) =>
                srcMember != null));

        CreateMap<Person, UpdatePersonalInformationDto>();
    }
}