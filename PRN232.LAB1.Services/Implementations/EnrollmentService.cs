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
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ExpandoObject>> GetEnrollmentsAsync(QueryParameters queryParams)
        {
            string includeProps = "";
            if (!string.IsNullOrEmpty(queryParams.Expand))
            {
                var expandList = new List<string>();
                if (queryParams.Expand.ToLower().Contains("student")) expandList.Add("Student");
                if (queryParams.Expand.ToLower().Contains("course")) expandList.Add("Course");
                includeProps = string.Join(",", expandList);
            }

            var query = await _unitOfWork.EnrollmentRepository.GetAllAsync(includeProperties: includeProps);
            var resultQuery = query.AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                var searchKeyword = queryParams.Search.ToLower();
                resultQuery = resultQuery.Where(s => s.Status.ToLower().Contains(searchKeyword));
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
                resultQuery = resultQuery.OrderBy(s => s.EnrollmentId);
            }

            int totalItems = resultQuery.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)queryParams.Size);

            var pagedData = resultQuery.Skip((queryParams.Page - 1) * queryParams.Size).Take(queryParams.Size).ToList();
            var mappedData = _mapper.Map<IEnumerable<EnrollmentModel>>(pagedData);
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

        public async Task<ExpandoObject?> GetEnrollmentByIdAsync(int id, string? fields = null, string? expand = null)
        {
            string includeProps = "";
            if (!string.IsNullOrEmpty(expand))
            {
                var expandList = new List<string>();
                if (expand.ToLower().Contains("student")) expandList.Add("Student");
                if (expand.ToLower().Contains("course")) expandList.Add("Course");
                includeProps = string.Join(",", expandList);
            }

            var entities = await _unitOfWork.EnrollmentRepository.GetAllAsync(s => s.EnrollmentId == id, includeProperties: includeProps);
            var entity = entities.FirstOrDefault();

            if (entity == null) return null;

            var mappedData = _mapper.Map<EnrollmentModel>(entity);
            return DataShaper.ShapeData(mappedData, fields ?? "");
        }

        public async Task<EnrollmentModel> CreateEnrollmentAsync(EnrollmentModel model)
        {
            var entity = _mapper.Map<Enrollment>(model);
            await _unitOfWork.EnrollmentRepository.InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<EnrollmentModel>(entity);
        }

        public async Task<EnrollmentModel?> UpdateEnrollmentAsync(int id, EnrollmentModel model)
        {
            var entities = await _unitOfWork.EnrollmentRepository.GetAllAsync(s => s.EnrollmentId == id);
            var entity = entities.FirstOrDefault();

            if (entity == null) return null;

            entity.StudentId = model.StudentId;
            entity.CourseId = model.CourseId;
            entity.EnrollDate = model.EnrollDate;
            entity.Status = model.Status;

            _unitOfWork.EnrollmentRepository.Update(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<EnrollmentModel>(entity);
        }

        public async Task<bool> DeleteEnrollmentAsync(int id)
        {
            var entities = await _unitOfWork.EnrollmentRepository.GetAllAsync(s => s.EnrollmentId == id);
            var entity = entities.FirstOrDefault();

            if (entity == null) return false;

            _unitOfWork.EnrollmentRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<PagedResponse<ExpandoObject>> GetEnrollmentsByCourseAsync(int courseId, QueryParameters queryParams)
        {
            string includeProps = "";
            if (!string.IsNullOrEmpty(queryParams.Expand))
            {
                var expandList = new List<string>();
                if (queryParams.Expand.ToLower().Contains("student")) expandList.Add("Student");
                if (queryParams.Expand.ToLower().Contains("course")) expandList.Add("Course");
                includeProps = string.Join(",", expandList);
            }

            var query = await _unitOfWork.EnrollmentRepository.GetAllAsync(e => e.CourseId == courseId, includeProperties: includeProps);
            var resultQuery = query.AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                var searchKeyword = queryParams.Search.ToLower();
                resultQuery = resultQuery.Where(s => s.Status.ToLower().Contains(searchKeyword));
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
                resultQuery = resultQuery.OrderBy(s => s.EnrollmentId);
            }

            int totalItems = resultQuery.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)queryParams.Size);

            var pagedData = resultQuery.Skip((queryParams.Page - 1) * queryParams.Size).Take(queryParams.Size).ToList();
            var mappedData = _mapper.Map<IEnumerable<EnrollmentModel>>(pagedData);
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
    }
}
