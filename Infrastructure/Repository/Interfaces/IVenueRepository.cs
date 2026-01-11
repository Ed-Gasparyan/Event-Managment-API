using Domain.Models;

namespace Infrastructure.Repository.Interfaces
{
    public interface IVenueRepository : IRepository<Venue>
    {
        Task<Venue?> GetVenueWithEventsAsync(int venueId);
        Task<int> CheckCapacityAsync(int venueId);
        Task<IEnumerable<Venue>> GetAvailableVenuesAsync(DateTime startTime, DateTime endTime);
    }
}
