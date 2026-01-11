using Application.DTOs.TicketDTO;

namespace Application.DTOs.UserDTOs
{
    public class UserTicketsDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<TicketResponseDto> Tickets { get; set; } = null!;
    }
}
