using Domain.Models;

namespace Infrastructure.Repository.Interfaces
{
    public interface ITicketRepository : IRepository<Ticket>
    {
        Task<IEnumerable<Ticket>> GetTicketsByUserAsync(int userId);
        Task<IEnumerable<Ticket>> GetTicketsByEventAsync(int eventId);
        Task<bool> CheckSeatAvailabilityAsync(int eventId, string seatNumber);
        Task<decimal> GetRevenuePerEventAsync(int eventId);
        Task<int> GetTotalTicketsSoldAsync(int eventId);
        Task PurchaseTicketAsync(Ticket ticket);
    }
}
