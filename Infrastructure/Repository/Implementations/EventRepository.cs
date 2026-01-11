using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace Infrastructure.Repository.Implementations
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<IEnumerable<Event>> GetEventsByVenueAsync(int venueId)
        {
            return await _dbSet
                .Where(x => x.VenueId == venueId)
                .Include(x => x.Venue)
                .ToListAsync();
        }

        public async Task<Event?> GetEventWithTicketsAsync(int eventId)
        {
            return await _dbSet
                .Where(x => x.Id == eventId)
                .Include(x => x.Tickets)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Event>> GetMostPopularEventsAsync(int topN)
        {
            return await _dbSet
                .OrderByDescending(x => x.Tickets.Count)
                .Take(topN)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            return await _dbSet
                .Where(x => x.StartTime >= DateTime.UtcNow)
                .OrderBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<(IEnumerable<Event> Events, int TotalCount)> GetEventsPaginatedAsync(int page, int pageSize)
        {
            var totalCount = await _dbSet.CountAsync();

            var events = await _dbSet
                .Include(e => e.Venue)
                .OrderBy(e => e.StartTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (events, totalCount);
        }
    }
}

