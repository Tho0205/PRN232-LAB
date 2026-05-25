using AutoMapper;
using PRN232.LAB1.Repositories.Entities;
using PRN232.LAB1.Services.Models.Business;

namespace PRN232.LAB1.Services.Mappers
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {

            CreateMap<StudentModel, Student>().ReverseMap();
            CreateMap<SemesterModel, Semester>().ReverseMap();
            CreateMap<CourseModel, Course>().ReverseMap();
            CreateMap<SubjectModel, Subject>().ReverseMap();
            CreateMap<EnrollmentModel, Enrollment>().ReverseMap();
        }
    }
}
