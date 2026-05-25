using AutoMapper;
using PRN232.LAB1.Repositories.Entities;
using PRN232.LAB1.Repositories.Interfaces;
using PRN232.LAB1.Services.Helpers;
using PRN232.LAB1.Services.Interfaces;
using PRN232.LAB1.Services.Models.Business;
using PRN232.LAB1.Services.Models.Requests;
using PRN232.LAB1.Services.Models.Responses;
using System.Dynamic;
using System.Linq.Dynamic.Core;

namespace PRN232.LAB1.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StudentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ExpandoObject>> GetStudentsAsync(QueryParameters queryParams)
        {
            string includeProps = "";
            if (!string.IsNullOrEmpty(queryParams.Expand) && queryParams.Expand.ToLower().Contains("enrollments"))
            {
                includeProps = "Enrollments";
            }

            var query = await _unitOfWork.StudentRepository.GetAllAsync(includeProperties: includeProps);
            var resultQuery = query.AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                var searchKeyword = queryParams.Search.ToLower();
                resultQuery = resultQuery.Where(s => s.FullName.ToLower().Contains(searchKeyword) || s.Email.ToLower().Contains(searchKeyword));
            }

            if (!string.IsNullOrEmpty(queryParams.Sort))
            {
                var sortParams = queryParams.Sort.Split(',');
                var sortStringList = new List<string>();
                foreach (var param in sortParams)
                {
                    var p = param.Trim();
                    if (p.StartsWith("-")) sortStringList.Add($"{p.Substring(1)} descending");
                    else sortStringList.Add($"{p} ascending");
                }
                var sortString = string.Join(", ", sortStringList);
                resultQuery = resultQuery.OrderBy(sortString);
            }
            else
            {
                resultQuery = resultQuery.OrderBy(s => s.StudentId);
            }

            int totalItems = resultQuery.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)queryParams.Size);

            var pagedData = resultQuery.Skip((queryParams.Page - 1) * queryParams.Size).Take(queryParams.Size).ToList();
            var mappedData = _mapper.Map<IEnumerable<StudentModel>>(pagedData);
            var shapedData = DataShaper.ShapeData(mappedData, queryParams.Fields ?? "");

            return new PagedResponse<ExpandoObject>
            {
                Items = shapedData,
                Pagination = new PaginationMetadata
                {
                    Page = queryParams.Page,
                    PageSize = queryParams.Size,
                    TotalItems = totalItems,
                    TotalPages = totalPages
                }
            };
        }

        public async Task<ExpandoObject?> GetStudentByIdAsync(int id, string? fields = null, string? expand = null)
        {
            string includeProps = "";
            if (!string.IsNullOrEmpty(expand) && expand.ToLower().Contains("enrollments"))
            {
                includeProps = "Enrollments";
            }

            var entities = await _unitOfWork.StudentRepository.GetAllAsync(s => s.StudentId == id, includeProperties: includeProps);
            var entity = entities.FirstOrDefault();

            if (entity == null) return null;

            var mappedData = _mapper.Map<StudentModel>(entity);
            return DataShaper.ShapeData(mappedData, fields ?? "");
        }

        public async Task<StudentModel> CreateStudentAsync(StudentModel model)
        {
            var entity = _mapper.Map<Student>(model);
            await _unitOfWork.StudentRepository.InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<StudentModel>(entity);
        }

        public async Task<StudentModel?> UpdateStudentAsync(int id, StudentModel model)
        {
            var entities = await _unitOfWork.StudentRepository.GetAllAsync(s => s.StudentId == id);
            var entity = entities.FirstOrDefault();

            if (entity == null) return null;

            entity.FullName = model.FullName;
            entity.Email = model.Email;
            entity.DateOfBirth = model.DateOfBirth;

            _unitOfWork.StudentRepository.Update(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<StudentModel>(entity);
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var entities = await _unitOfWork.StudentRepository.GetAllAsync(s => s.StudentId == id);
            var entity = entities.FirstOrDefault();

            if (entity == null) return false;

            _unitOfWork.StudentRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
