using Application.DTOs.EventDTOs;
using Application.DTOs.UserDTOs;

namespace Application.DTOs.TicketDTO
{
    public class TicketResponseDto
    {
        public int Id { get; set; }
        public string SeatNumber { get; set; } = null!;
        public DateTime PurchasedAt { get; set; }
        public decimal Price { get; set; }
        public UserResponseDto User { get; set; } = null!;
        public EventResponseDto Event { get; set; } = null!;
    }
}
