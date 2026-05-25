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
            try
            {
                var result = await _studentService.GetStudentsAsync(queryParams);
                return Success(result.Items, "Students retrieved successfully", 200, result.Pagination);
            }
            catch (Exception ex)
            {
                return Error(ex.Message, null, 500);
            }
        }

        [HttpGet("{id}")]
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

        [HttpPut("{id}")]
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var isDeleted = await _studentService.DeleteStudentAsync(id);
            if (!isDeleted)
            {
                return Error($"Student with ID {id} not found", null, 404);
            }

            return Success<object>(null, "Student deleted successfully", 200);
        }
    }
}
