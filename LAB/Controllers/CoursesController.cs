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
    [Route("api/v{version:apiVersion}/courses")]
    public class CoursesController : BaseController
    {
        private readonly ICourseService _courseService;
        private readonly IEnrollmentService _enrollmentService;
        private readonly IMapper _mapper;

        public CoursesController(ICourseService courseService, IEnrollmentService enrollmentService, IMapper mapper)
        {
            _courseService = courseService;
            _enrollmentService = enrollmentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCourses([FromQuery] QueryParameters queryParams)
        {
            var result = await _courseService.GetCoursesAsync(queryParams);
            return Success(result.Items, "Courses retrieved successfully", 200, result.Pagination);
        }

        [HttpGet("{id:int}", Name = "GetCourseById")]
        public async Task<IActionResult> GetCourseById(int id, [FromQuery] string? fields, [FromQuery] string? expand)
        {
            var result = await _courseService.GetCourseByIdAsync(id, fields, expand);
            if (result == null)
            {
                return Error($"Course with ID {id} not found", null, 404);
            }
            return Success(result, "Course retrieved successfully");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Error("Invalid input data", ModelState, 400);
            }

            var model = _mapper.Map<CourseModel>(request);
            var createdModel = await _courseService.CreateCourseAsync(model);
            var response = _mapper.Map<CourseResponse>(createdModel);
            
            return Success(response, "Course created successfully", 201);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Error("Invalid input data", ModelState, 400);
            }

            var model = _mapper.Map<CourseModel>(request);
            var updatedModel = await _courseService.UpdateCourseAsync(id, model);

            if (updatedModel == null)
            {
                return Error($"Course with ID {id} not found", null, 404);
            }

            var response = _mapper.Map<CourseResponse>(updatedModel);
            return Success(response, "Course updated successfully", 200);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var isDeleted = await _courseService.DeleteCourseAsync(id);
            if (!isDeleted)
            {
                return Error($"Course with ID {id} not found", null, 404);
            }

            return Success<object?>(null, "Course deleted successfully", 200);
        }

        [HttpGet("{id:int}/enrollments", Name = "GetEnrollmentsByCourse")]
        public async Task<IActionResult> GetEnrollmentsByCourse(int id, [FromQuery] QueryParameters queryParams)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return Error($"Course with ID {id} not found", null, 404);
            }

            var result = await _enrollmentService.GetEnrollmentsByCourseAsync(id, queryParams);
            return Success(result.Items, "Enrollments retrieved successfully", 200, result.Pagination);
        }
    }
}
