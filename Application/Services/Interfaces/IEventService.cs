using Application.DTOs.EventDTOs;
using Application.Utilities;

namespace Application.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventResponseDto> CreateEventAsync(EventDto eventDto);
        Task<EventResponseDto> UpdateEventAsync(int eventId, EventDto eventDto);
        Task DeleteEventAsync(int eventId);
        Task<EventWithTicketsDto> GetEventWithTicketsAsync(int eventId);
        Task<IEnumerable<EventResponseDto>> GetUpcomingEventsAsync();
        Task<EventPaginatedList> GetEventsPaginatedAsync(int page, int pageSize, string? venueName = null, string? sortBy = null);
        Task<IEnumerable<EventResponseDto>> GetMostPopularEventsAsync(int topN);
    }

}
