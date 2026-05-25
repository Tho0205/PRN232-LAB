using AutoMapper;
using PRN232.LAB1.API.Models.Requests;
using PRN232.LAB1.API.Models.Responses;
using PRN232.LAB1.Services.Models.Business;

namespace PRN232.LAB1.API.Mappers
{
    public class ApiMappingProfile : Profile
    {
        public ApiMappingProfile()
        {

            CreateMap<StudentCreateRequest, StudentModel>();
            CreateMap<SemesterCreateRequest, SemesterModel>();
            CreateMap<CourseCreateRequest, CourseModel>();
            CreateMap<SubjectCreateRequest, SubjectModel>();
            CreateMap<EnrollmentCreateRequest, EnrollmentModel>();


            CreateMap<StudentModel, StudentResponse>();
            CreateMap<SemesterModel, SemesterResponse>();
            CreateMap<CourseModel, CourseResponse>();
            CreateMap<SubjectModel, SubjectResponse>();
            CreateMap<EnrollmentModel, EnrollmentResponse>();
        }
    }
}
