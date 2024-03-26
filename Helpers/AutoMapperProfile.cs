using AutoMapper;
using school_management_backend.Models;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Login, Users>();
        CreateMap<Login, Teacher>();
        CreateMap<Login, Student>();
    }
}
