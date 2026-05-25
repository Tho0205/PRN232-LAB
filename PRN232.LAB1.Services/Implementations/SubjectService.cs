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
    public class SubjectService : ISubjectService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubjectService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<PagedResponse<ExpandoObject>> GetSubjectsAsync(QueryParameters queryParams)
        {
            var query = await _unitOfWork.SubjectRepository.GetAllAsync();
            var resultQuery = query.AsQueryable();

            if (!string.IsNullOrEmpty(queryParams.Search))
            {
                var searchKeyword = queryParams.Search.ToLower();
                resultQuery = resultQuery.Where(s => s.SubjectName.ToLower().Contains(searchKeyword) || s.SubjectCode.ToLower().Contains(searchKeyword));
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
                resultQuery = resultQuery.OrderBy(s => s.SubjectId);
            }

            int totalItems = resultQuery.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)queryParams.Size);

            var pagedData = resultQuery.Skip((queryParams.Page - 1) * queryParams.Size).Take(queryParams.Size).ToList();
            var mappedData = _mapper.Map<IEnumerable<SubjectModel>>(pagedData);
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

        public async Task<ExpandoObject?> GetSubjectByIdAsync(int id, string? fields = null)
        {
            var entities = await _unitOfWork.SubjectRepository.GetAllAsync(s => s.SubjectId == id);
            var entity = entities.FirstOrDefault();

            if (entity == null) return null;

            var mappedData = _mapper.Map<SubjectModel>(entity);
            return DataShaper.ShapeData(mappedData, fields ?? "");
        }

        public async Task<SubjectModel> CreateSubjectAsync(SubjectModel model)
        {
            var entity = _mapper.Map<Subject>(model);
            await _unitOfWork.SubjectRepository.InsertAsync(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<SubjectModel>(entity);
        }

        public async Task<SubjectModel?> UpdateSubjectAsync(int id, SubjectModel model)
        {
            var entities = await _unitOfWork.SubjectRepository.GetAllAsync(s => s.SubjectId == id);
            var entity = entities.FirstOrDefault();

            if (entity == null) return null;

            entity.SubjectCode = model.SubjectCode;
            entity.SubjectName = model.SubjectName;
            entity.Credit = model.Credit;

            _unitOfWork.SubjectRepository.Update(entity);
            await _unitOfWork.SaveAsync();

            return _mapper.Map<SubjectModel>(entity);
        }

        public async Task<bool> DeleteSubjectAsync(int id)
        {
            var entities = await _unitOfWork.SubjectRepository.GetAllAsync(s => s.SubjectId == id);
            var entity = entities.FirstOrDefault();

            if (entity == null) return false;

            _unitOfWork.SubjectRepository.Delete(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
