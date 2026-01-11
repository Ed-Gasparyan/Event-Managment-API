namespace Application.DTOs.EventDTOs
{
    public class EventDto
    {
        public string Title { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int VenueId { get; set; }
        public decimal Price { get; set; }
    }
}
