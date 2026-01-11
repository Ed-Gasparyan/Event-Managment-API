using Application.DTOs.TicketDTO;

namespace Application.Services.Interfaces
{
    public interface ITicketService
    {
        Task<TicketResponseDto> PurchaseTicketAsync(TicketDto ticketDto);
        Task<IEnumerable<TicketResponseDto>> GetTicketsByUserAsync(int userId);
        Task<IEnumerable<TicketResponseDto>> GetTicketsByEventAsync(int eventId);
        Task<bool> CheckSeatAvailabilityAsync(int eventId, string seatNumber);
        Task<decimal> GetRevenuePerEventAsync(int eventId);
        Task<int> GetTotalTicketsSoldAsync(int eventId);
    }
}
