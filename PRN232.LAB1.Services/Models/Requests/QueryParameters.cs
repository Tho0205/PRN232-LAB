namespace PRN232.LAB1.Services.Models.Requests
{
    public class QueryParameters
    {
        public string? Search { get; set; }
        public string? Sort { get; set; }
        public string? Fields { get; set; }
        public string? Expand { get; set; }

        private const int maxPageSize = 50;
        public int Page { get; set; } = 1;

        private int _size = 10;
        public int Size
        {
            get { return _size; }
            set { _size = (value > maxPageSize) ? maxPageSize : value; }
        }
    }
}
