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
    public class EnrollmentsController : BaseController
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IMapper _mapper;

        public EnrollmentsController(IEnrollmentService enrollmentService, IMapper mapper)
        {
            _enrollmentService = enrollmentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetEnrollments([FromQuery] QueryParameters queryParams)
        {
            try
            {
                var result = await _enrollmentService.GetEnrollmentsAsync(queryParams);
                return Success(result.Items, "Enrollments retrieved successfully", 200, result.Pagination);
            }
            catch (Exception ex)
            {
                return Error(ex.Message, null, 500);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEnrollmentById(int id, [FromQuery] string? fields, [FromQuery] string? expand)
        {
            var result = await _enrollmentService.GetEnrollmentByIdAsync(id, fields, expand);
            if (result == null)
            {
                return Error($"Enrollment with ID {id} not found", null, 404);
            }
            return Success(result, "Enrollment retrieved successfully");
        }

        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] EnrollmentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Error("Invalid input data", ModelState, 400);
            }

            var model = _mapper.Map<EnrollmentModel>(request);
            var createdModel = await _enrollmentService.CreateEnrollmentAsync(model);
            var response = _mapper.Map<EnrollmentResponse>(createdModel);
            
            return Success(response, "Enrollment created successfully", 201);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrollment(int id, [FromBody] EnrollmentCreateRequest request)
        {
            if (!ModelState.IsValid)
            {
                return Error("Invalid input data", ModelState, 400);
            }

            var model = _mapper.Map<EnrollmentModel>(request);
            var updatedModel = await _enrollmentService.UpdateEnrollmentAsync(id, model);

            if (updatedModel == null)
            {
                return Error($"Enrollment with ID {id} not found", null, 404);
            }

            var response = _mapper.Map<EnrollmentResponse>(updatedModel);
            return Success(response, "Enrollment updated successfully", 200);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var isDeleted = await _enrollmentService.DeleteEnrollmentAsync(id);
            if (!isDeleted)
            {
                return Error($"Enrollment with ID {id} not found", null, 404);
            }

            return Success<object>(null, "Enrollment deleted successfully", 200);
        }
    }
}
