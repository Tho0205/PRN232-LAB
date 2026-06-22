using AutoMapper;
using PRN232.LAB.Repositories.Entities;
using PRN232.LAB.Repositories.Interfaces;
using PRN232.LAB.Services.Helpers;
using PRN232.LAB.Services.Interfaces;
using PRN232.LAB.Services.Models.Business;
using PRN232.LAB.Services.Models.Requests;
using PRN232.LAB.Services.Models.Responses;
using System.Dynamic;
using System.Linq.Dynamic.Core;

namespace PRN232.LAB.Services.Implementations
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ExpandoObject>> GetCoursesAsync(QueryParameters queryParams)
        {
            string includeProps = "";
            if (!string.IsNullOrEmpty(queryParams.Expand))
            {
                var expandList = new List<string>();
                if (queryParams.Expand.ToLower().Contains("semester")) expandList.Add("Semester");
                if (queryParams.Expand.ToLower().Contains("enrollments")) expandList.Add("Enrollments");
                includeProps = string.Join(",", expandList);
            }

            var query = await _unitOfWork.CourseRepository.GetAllAsync(includeProperties: includeProps);
            var resultQuery = query.AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                var searchKeyword = queryParams.Search.ToLower();
                resultQuery = resultQuery.Where(s => s.CourseName.ToLower().Contains(searchKeyword));
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
                resultQuery = resultQuery.OrderBy(s => s.CourseId);
            }

            int totalItems = resultQuery.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)queryParams.Size);

            var pagedData = resultQuery.Skip((queryParams.Page - 1) * queryParams.Size).Take(queryParams.Size).ToList();
            var mappedData = _mapper.Map<IEnumerable<CourseModel>>(pagedData);
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

        public async Task<ExpandoObject?> GetCourseByIdAsync(int id, string? fields = null, string? expand = null)
        {
            var includeProps = "Semester,Enrollments,Enrollments.Student";

            var entities = await _unitOfWork.CourseRepository.GetAllAsync(s => s.CourseId == id, includeProperties: includeProps);
            var entity = entities.FirstOrDefault();

            if (entity == null) return null;

            if (entity.Semester != null)
            {
                entity.Semester.Courses = new List<Course>();
            }

            foreach (var enrollment in entity.Enrollments)
            {
                enrollment.Course = null!;
                if (enrollment.Student != null)
                {
                    enrollment.Student.Enrollments = new List<Enrollment>();
                }
            }

            var mappedData = _mapper.Map<CourseModel>(entity);
            return DataShaper.ShapeData(mappedData, fields ?? "");
        }

        public async Task<CourseModel> CreateCourseAsync(CourseModel model)
        {
            var entity = _mapper.Map<Course>(model);
            await _unitOfWork.CourseRepository.InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<CourseModel>(entity);
        }

        public async Task<CourseModel?> UpdateCourseAsync(int id, CourseModel model)
        {
            var entities = await _unitOfWork.CourseRepository.GetAllAsync(s => s.CourseId == id);
            var entity = entities.FirstOrDefault();

            if (entity == null) return null;

            entity.CourseName = model.CourseName;
            entity.SemesterId = model.SemesterId;

            _unitOfWork.CourseRepository.Update(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<CourseModel>(entity);
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            var entities = await _unitOfWork.CourseRepository.GetAllAsync(s => s.CourseId == id);
            var entity = entities.FirstOrDefault();

            if (entity == null) return false;

            _unitOfWork.CourseRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
