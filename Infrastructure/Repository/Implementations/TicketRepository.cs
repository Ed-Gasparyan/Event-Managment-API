using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementations
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        public TicketRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        public async Task<bool> CheckSeatAvailabilityAsync(int eventId, string seatNumber)
        {
            var ticket = await _dbSet.FirstOrDefaultAsync(x => x.EventId == eventId && x.SeatNumber == seatNumber);

            return ticket == null;
        }

        public async Task<decimal> GetRevenuePerEventAsync(int eventId)
        {
            return await _dbSet
                .Where(t => t.EventId == eventId)
                .SumAsync(t => t.Price);            
        }

        public async Task<IEnumerable<Ticket>> GetTicketsByEventAsync(int eventId)
        {
            return await _dbSet
                .Where(t => t.EventId == eventId)
                .Include(t => t.Event)              
                .ToListAsync();                     
        }


        public async Task<IEnumerable<Ticket>> GetTicketsByUserAsync(int userId)
        {
            return await _dbSet
                .Where(x => x.UserId == userId)
                .ToListAsync();
        }

        public async Task<int> GetTotalTicketsSoldAsync(int eventId)
        {
            return await _dbSet
                .Where(x => x.EventId == eventId)
                .CountAsync();
        }

        public async Task PurchaseTicketAsync(Ticket ticket)
        {
            // Set purchase time (business meaning)
            ticket.PurchasedAt = DateTime.UtcNow;

            _dbSet.Add(ticket);

            await _context.SaveChangesAsync();
        }
    }
}
