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
    [Route("api/v{version:apiVersion}/students")]
    public class StudentsController : BaseController
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public StudentsController(IStudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetStudents([FromQuery] QueryParameters queryParams)
        {
            var result = await _studentService.GetStudentsAsync(queryParams);
            return Success(result.Items, "Students retrieved successfully", 200, result.Pagination);
        }

        [HttpGet("{id:int}", Name = "GetStudentByIdV1")]
        public async Task<IActionResult> GetStudentById(int id, [FromQuery] string? fields, [FromQuery] string? expand)
        {
            var result = await _studentService.GetStudentByIdAsync(id, fields, expand);
            if (result == null)
            {
                return Error($"Student with ID {id} not found", null, 404);
            }
            

            return Success(result, "Student retrieved successfully");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Error("Invalid input data", ModelState, 400);
            }

            var model = _mapper.Map<StudentModel>(request);
            var createdModel = await _studentService.CreateStudentAsync(model);
            var response = _mapper.Map<StudentResponse>(createdModel);
            
            return Success(response, "Student created successfully", 201);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Error("Invalid input data", ModelState, 400);
            }

            var model = _mapper.Map<StudentModel>(request);
            var updatedModel = await _studentService.UpdateStudentAsync(id, model);

            if (updatedModel == null)
            {
                return Error($"Student with ID {id} not found", null, 404);
            }

            var response = _mapper.Map<StudentResponse>(updatedModel);
            return Success(response, "Student updated successfully", 200);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var isDeleted = await _studentService.DeleteStudentAsync(id);
            if (!isDeleted)
            {
                return Error($"Student with ID {id} not found", null, 404);
            }

            return Success<object?>(null, "Student deleted successfully", 200);
        }
    }
}
