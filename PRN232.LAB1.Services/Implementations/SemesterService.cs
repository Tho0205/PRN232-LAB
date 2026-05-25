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
    public class SemesterService : ISemesterService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SemesterService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ExpandoObject>> GetSemestersAsync(QueryParameters queryParams)
        {
            string includeProps = "";
            if (!string.IsNullOrEmpty(queryParams.Expand) && queryParams.Expand.ToLower().Contains("courses"))
            {
                includeProps = "Courses";
            }

            var query = await _unitOfWork.SemesterRepository.GetAllAsync(includeProperties: includeProps);
            var resultQuery = query.AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                var searchKeyword = queryParams.Search.ToLower();
                resultQuery = resultQuery.Where(s => s.SemesterName.ToLower().Contains(searchKeyword));
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
                resultQuery = resultQuery.OrderBy(s => s.SemesterId);
            }

            int totalItems = resultQuery.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)queryParams.Size);

            var pagedData = resultQuery.Skip((queryParams.Page - 1) * queryParams.Size).Take(queryParams.Size).ToList();
            var mappedData = _mapper.Map<IEnumerable<SemesterModel>>(pagedData);
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

        public async Task<ExpandoObject?> GetSemesterByIdAsync(int id, string? fields = null, string? expand = null)
        {
            string includeProps = "";
            if (!string.IsNullOrEmpty(expand) && expand.ToLower().Contains("courses"))
            {
                includeProps = "Courses";
            }

            var entities = await _unitOfWork.SemesterRepository.GetAllAsync(s => s.SemesterId == id, includeProperties: includeProps);
            var entity = entities.FirstOrDefault();

            if (entity == null) return null;

            var mappedData = _mapper.Map<SemesterModel>(entity);
            return DataShaper.ShapeData(mappedData, fields ?? "");
        }

        public async Task<SemesterModel> CreateSemesterAsync(SemesterModel model)
        {
            var entity = _mapper.Map<Semester>(model);
            await _unitOfWork.SemesterRepository.InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<SemesterModel>(entity);
        }

        public async Task<SemesterModel?> UpdateSemesterAsync(int id, SemesterModel model)
        {
            var entities = await _unitOfWork.SemesterRepository.GetAllAsync(s => s.SemesterId == id);
            var entity = entities.FirstOrDefault();

            if (entity == null) return null;

            entity.SemesterName = model.SemesterName;
            entity.StartDate = model.StartDate;
            entity.EndDate = model.EndDate;

            _unitOfWork.SemesterRepository.Update(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<SemesterModel>(entity);
        }

        public async Task<bool> DeleteSemesterAsync(int id)
        {
            var entities = await _unitOfWork.SemesterRepository.GetAllAsync(s => s.SemesterId == id);
            var entity = entities.FirstOrDefault();

            if (entity == null) return false;

            _unitOfWork.SemesterRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
