using PRN232.LAB.Services.Models.Business;
using PRN232.LAB.Services.Models.Requests;
using PRN232.LAB.Services.Models.Responses;
using System.Dynamic;

namespace PRN232.LAB.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<PagedResponse<ExpandoObject>> GetSubjectsAsync(QueryParameters queryParams);
        Task<ExpandoObject?> GetSubjectByIdAsync(int id, string? fields = null);
        Task<SubjectModel> CreateSubjectAsync(SubjectModel model);
        Task<SubjectModel?> UpdateSubjectAsync(int id, SubjectModel model);
        Task<bool> DeleteSubjectAsync(int id);
    }
}
