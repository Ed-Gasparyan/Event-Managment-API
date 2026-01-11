namespace Application.DTOs.EventDTOs
{
    public class EventFilterDto
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }
        public string VenueName { get; set; } = null!;
        public string SortBy { get; set; } = null!;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
