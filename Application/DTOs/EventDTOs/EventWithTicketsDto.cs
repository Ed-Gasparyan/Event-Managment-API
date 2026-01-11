using Application.DTOs.TicketDTO;
using Application.DTOs.VenueDTOs;

namespace Application.DTOs.EventDTOs
{
    public class EventWithTicketsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public VenueResponseDto Venue { get; set; } = null!;
        public IEnumerable<TicketResponseDto> Tickets { get; set; } = null!;
    }
}
