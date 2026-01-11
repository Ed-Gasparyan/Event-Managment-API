using Domain.Models;
namespace Infrastructure.Repository.Interfaces
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<Event?> GetEventWithTicketsAsync(int eventId);
        Task<IEnumerable<Event>> GetEventsByVenueAsync(int venueId);
        Task<IEnumerable<Event>> GetUpcomingEventsAsync();
        Task<IEnumerable<Event>> GetMostPopularEventsAsync(int topN);
        Task<(IEnumerable<Event> Events, int TotalCount)> GetEventsPaginatedAsync(int page, int pageSize);
    }
}
