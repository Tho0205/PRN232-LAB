using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LAB.API.Models.Requests;
using PRN232.LAB.API.Models.Responses;
using PRN232.LAB.Services.Interfaces;
using PRN232.LAB.Services.Models.Business;
using PRN232.LAB.Services.Models.Requests;
using PRN232.LAB.Services.Models.Responses;

namespace PRN232.LAB.API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v{version:apiVersion}/semesters")]
    public class SemestersController : BaseController
    {
        private readonly ISemesterService _semesterService;
        private readonly IMapper _mapper;

        public SemestersController(ISemesterService semesterService, IMapper mapper)
        {
            _semesterService = semesterService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetSemesters([FromQuery] QueryParameters queryParams)
        {
            var result = await _semesterService.GetSemestersAsync(queryParams);
            return Success(result.Items, "Semesters retrieved successfully", 200, result.Pagination);
        }

        [HttpGet("{id:int}", Name = "GetSemesterById")]
        public async Task<IActionResult> GetSemesterById(int id, [FromQuery] string? fields, [FromQuery] string? expand)
        {
            var result = await _semesterService.GetSemesterByIdAsync(id, fields, expand);
            if (result == null)
            {
                return Error($"Semester with ID {id} not found", null, 404);
            }
            return Success(result, "Semester retrieved successfully");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSemester([FromBody] SemesterCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Error("Invalid input data", ModelState, 400);
            }

            var model = _mapper.Map<SemesterModel>(request);
            var createdModel = await _semesterService.CreateSemesterAsync(model);
            var response = _mapper.Map<SemesterResponse>(createdModel);
            
            return Success(response, "Semester created successfully", 201);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSemester(int id, [FromBody] SemesterCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Error("Invalid input data", ModelState, 400);
            }

            var model = _mapper.Map<SemesterModel>(request);
            var updatedModel = await _semesterService.UpdateSemesterAsync(id, model);

            if (updatedModel == null)
            {
                return Error($"Semester with ID {id} not found", null, 404);
            }

            var response = _mapper.Map<SemesterResponse>(updatedModel);
            return Success(response, "Semester updated successfully", 200);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSemester(int id)
        {
            var isDeleted = await _semesterService.DeleteSemesterAsync(id);
            if (!isDeleted)
            {
                return Error($"Semester with ID {id} not found", null, 404);
            }

            return Success<object?>(null, "Semester deleted successfully", 200);
        }
    }
}
