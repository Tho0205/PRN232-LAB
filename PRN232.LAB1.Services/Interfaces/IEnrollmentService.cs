using PRN232.LAB1.Services.Models.Business;
using PRN232.LAB1.Services.Models.Requests;
using PRN232.LAB1.Services.Models.Responses;
using System.Dynamic;

namespace PRN232.LAB1.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<PagedResponse<ExpandoObject>> GetEnrollmentsAsync(QueryParameters queryParams);
        Task<ExpandoObject?> GetEnrollmentByIdAsync(int id, string? fields = null, string? expand = null);
        Task<EnrollmentModel> CreateEnrollmentAsync(EnrollmentModel model);
        Task<EnrollmentModel?> UpdateEnrollmentAsync(int id, EnrollmentModel model);
        Task<bool> DeleteEnrollmentAsync(int id);
        Task<PagedResponse<ExpandoObject>> GetEnrollmentsByCourseAsync(int courseId, QueryParameters queryParams);
    }
}
