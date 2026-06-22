using Microsoft.AspNetCore.Mvc;
using PRN232.LAB.Services.Models.Responses;

namespace PRN232.LAB.API.Controllers
{
    public class BaseController : ControllerBase
    {
        protected IActionResult Success<T>(T data, string message = "Request processed successfully", int statusCode = 200, PaginationMetadata? pagination = null)
        {
            var response = new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                Errors = null,
                Pagination = pagination
            };
            return StatusCode(statusCode, response);
        }

        protected IActionResult Error(string message, object? errors = null, int statusCode = 400)
        {
            var response = new ApiResponse<object>
            {
                Success = false,
                Message = message,
                Data = null,
                Errors = errors
            };
            return StatusCode(statusCode, response);
        }
    }
}
