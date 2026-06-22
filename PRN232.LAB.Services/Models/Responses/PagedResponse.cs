namespace PRN232.LAB.Services.Models.Responses
{
    public class PagedResponse<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public PaginationMetadata Pagination { get; set; } = null!;
    }
}
