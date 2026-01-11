using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository.Implementations
{
    public class VenueRepository : Repository<Venue>, IVenueRepository
    {

        public VenueRepository(ApplicationDbContext context) : base(context)
        {
            
        }

        public async Task<int> CheckCapacityAsync(int venueId)
        {
            var entity = await _dbSet
                .FirstOrDefaultAsync(x => x.Id == venueId);

            if (entity == null)
            {
                return 0;
            }
            return entity.Capacity;
        }

        public async Task<IEnumerable<Venue>> GetAvailableVenuesAsync(DateTime startTime, DateTime endTime)
        {
            return await _dbSet
                .Where(v => !_context.Events
                    .Any(e => e.VenueId == v.Id &&
                              e.StartTime < endTime &&
                              e.EndTime > startTime))
                .ToListAsync();
        }

        public async Task<Venue?> GetVenueWithEventsAsync(int venueId)
        {
            return await _dbSet
                .Where(x => x.Id == venueId)
                .Include(x => x.Events)
                .FirstOrDefaultAsync();
        }
    }
}
