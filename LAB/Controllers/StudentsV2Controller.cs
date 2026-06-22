using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LAB.API.Models.Requests;
using PRN232.LAB.API.Models.Responses;
using PRN232.LAB.Services.Interfaces;
using PRN232.LAB.Services.Models.Business;
using PRN232.LAB.Services.Models.Requests;

namespace PRN232.LAB.API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("api/v{version:apiVersion}/students")]
    public class StudentsV2Controller : BaseController
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public StudentsV2Controller(IStudentService studentService, IMapper mapper)
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

        [HttpGet("{id:int}", Name = "GetStudentByIdV2")]
        public async Task<IActionResult> GetStudentById([FromRoute] int id, [FromQuery] string? fields, [FromQuery] string? expand)
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
    }
}
