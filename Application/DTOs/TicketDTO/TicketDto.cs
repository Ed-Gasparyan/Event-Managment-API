namespace Application.DTOs.TicketDTO
{
    public class TicketDto
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public decimal Price { get; set; }
    }
}
