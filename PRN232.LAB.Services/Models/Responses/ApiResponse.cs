namespace PRN232.LAB.Services.Models.Responses
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public T? Data { get; set; }
        public PaginationMetadata? Pagination { get; set; }
        public object? Errors { get; set; }
    }
}
