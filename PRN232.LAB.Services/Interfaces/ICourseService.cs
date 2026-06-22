using PRN232.LAB.Services.Models.Business;
using PRN232.LAB.Services.Models.Requests;
using PRN232.LAB.Services.Models.Responses;
using System.Dynamic;

namespace PRN232.LAB.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PagedResponse<ExpandoObject>> GetCoursesAsync(QueryParameters queryParams);
        Task<ExpandoObject?> GetCourseByIdAsync(int id, string? fields = null, string? expand = null);
        Task<CourseModel> CreateCourseAsync(CourseModel model);
        Task<CourseModel?> UpdateCourseAsync(int id, CourseModel model);
        Task<bool> DeleteCourseAsync(int id);
    }
}
