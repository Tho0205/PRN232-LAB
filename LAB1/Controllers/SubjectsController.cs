using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PRN232.LAB1.API.Models.Requests;
using PRN232.LAB1.API.Models.Responses;
using PRN232.LAB1.Services.Interfaces;
using PRN232.LAB1.Services.Models.Business;
using PRN232.LAB1.Services.Models.Requests;
using PRN232.LAB1.Services.Models.Responses;

namespace PRN232.LAB1.API.Controllers
{
    public class SubjectsController : BaseController
    {
        private readonly ISubjectService _subjectService;
        private readonly IMapper _mapper;

        public SubjectsController(ISubjectService subjectService, IMapper mapper)
        {
            _subjectService = subjectService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetSubjects([FromQuery] QueryParameters queryParams)
        {
            try
            {
                var result = await _subjectService.GetSubjectsAsync(queryParams);
                return Success(result.Items, "Subjects retrieved successfully", 200, result.Pagination);
            }
            catch (Exception ex)
            {
                return Error(ex.Message, null, 500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectById(int id, [FromQuery] string? fields)
        {
            var result = await _subjectService.GetSubjectByIdAsync(id, fields);
            if (result == null)
            {
                return Error($"Subject with ID {id} not found", null, 404);
            }
            return Success(result, "Subject retrieved successfully");
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Error("Invalid input data", ModelState, 400);
            }

            var model = _mapper.Map<SubjectModel>(request);
            var createdModel = await _subjectService.CreateSubjectAsync(model);
            var response = _mapper.Map<SubjectResponse>(createdModel);
            
            return Success(response, "Subject created successfully", 201);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] SubjectCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Error("Invalid input data", ModelState, 400);
            }

            var model = _mapper.Map<SubjectModel>(request);
            var updatedModel = await _subjectService.UpdateSubjectAsync(id, model);

            if (updatedModel == null)
            {
                return Error($"Subject with ID {id} not found", null, 404);
            }

            var response = _mapper.Map<SubjectResponse>(updatedModel);
            return Success(response, "Subject updated successfully", 200);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var isDeleted = await _subjectService.DeleteSubjectAsync(id);
            if (!isDeleted)
            {
                return Error($"Subject with ID {id} not found", null, 404);
            }

            return Success<object>(null, "Subject deleted successfully", 200);
        }
    }
}
