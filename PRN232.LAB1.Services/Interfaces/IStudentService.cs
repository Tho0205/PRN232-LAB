using PRN232.LAB1.Services.Models.Business;
using PRN232.LAB1.Services.Models.Requests;
using PRN232.LAB1.Services.Models.Responses;
using System.Dynamic;

namespace PRN232.LAB1.Services.Interfaces
{
    public interface IStudentService
    {
        Task<PagedResponse<ExpandoObject>> GetStudentsAsync(QueryParameters queryParams);
        Task<ExpandoObject?> GetStudentByIdAsync(int id, string? fields = null, string? expand = null);
        Task<StudentModel> CreateStudentAsync(StudentModel model);
        Task<StudentModel?> UpdateStudentAsync(int id, StudentModel model);
        Task<bool> DeleteStudentAsync(int id);
    }
}
