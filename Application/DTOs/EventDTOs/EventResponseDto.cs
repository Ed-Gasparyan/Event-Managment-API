using Application.DTOs.VenueDTOs;

namespace Application.DTOs.EventDTOs
{
    public class EventResponseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public VenueResponseDto Venue { get; set; } = null!;
    }
}
