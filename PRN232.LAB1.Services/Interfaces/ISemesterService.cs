using PRN232.LAB1.Services.Models.Business;
using PRN232.LAB1.Services.Models.Requests;
using PRN232.LAB1.Services.Models.Responses;
using System.Dynamic;

namespace PRN232.LAB1.Services.Interfaces
{
    public interface ISemesterService
    {
        Task<PagedResponse<ExpandoObject>> GetSemestersAsync(QueryParameters queryParams);
        Task<ExpandoObject?> GetSemesterByIdAsync(int id, string? fields = null, string? expand = null);
        Task<SemesterModel> CreateSemesterAsync(SemesterModel model);
        Task<SemesterModel?> UpdateSemesterAsync(int id, SemesterModel model);
        Task<bool> DeleteSemesterAsync(int id);
    }
}
