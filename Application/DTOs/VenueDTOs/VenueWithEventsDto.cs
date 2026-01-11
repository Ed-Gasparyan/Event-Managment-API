using Application.DTOs.EventDTOs;

namespace Application.DTOs.VenueDTOs
{
    public class VenueWithEventsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public int Capacity { get; set; }
        public IEnumerable<EventResponseDto> Events { get; set; } = null!;
    }
}
