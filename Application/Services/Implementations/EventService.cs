using Application.DTOs.EventDTOs;
using Application.DTOs.TicketDTO;
using Application.DTOs.UserDTOs;
using Application.DTOs.VenueDTOs;
using Application.Services.Interfaces;
using Application.Utilities;
using Domain.Models;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IVenueRepository _venueRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<EventService> _logger;

        public EventService(
            IEventRepository eventRepository,
            IVenueRepository venueRepository,
            ITicketRepository ticketRepository,
            IUserRepository userRepository,
            ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _venueRepository = venueRepository;
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        // Create a new event
        public async Task<EventResponseDto> CreateEventAsync(EventDto eventDto)
        {
            try
            {
                var venue = await _venueRepository.GetByIdAsync(eventDto.VenueId);
                if (venue == null)
                    throw new InvalidOperationException($"Venue with Id {eventDto.VenueId} not found.");

                var ev = new Event
                {
                    Title = eventDto.Title,
                    StartTime = eventDto.StartTime,
                    EndTime = eventDto.EndTime,
                    VenueId = eventDto.VenueId,
                    Venue = venue,
                    Tickets = new List<Ticket>()
                };

                await _eventRepository.AddAsync(ev);
                return MapEventToDto(ev, venue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateEventAsync {@EventDto}", eventDto);
                throw;
            }
        }

        // Update existing event
        public async Task<EventResponseDto> UpdateEventAsync(int eventId, EventDto eventDto)
        {
            try
            {
                var ev = await _eventRepository.GetByIdAsync(eventId);
                if (ev == null)
                    throw new InvalidOperationException($"Event with Id {eventId} not found.");

                var venue = await _venueRepository.GetByIdAsync(eventDto.VenueId);
                if (venue == null)
                    throw new InvalidOperationException($"Venue with Id {eventDto.VenueId} not found.");

                ev.Title = eventDto.Title;
                ev.StartTime = eventDto.StartTime;
                ev.EndTime = eventDto.EndTime;
                ev.VenueId = eventDto.VenueId;
                ev.Venue = venue;

                await _eventRepository.DeleteAsync(eventId);
                return MapEventToDto(ev, venue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, eventId, eventDto);
                throw;
            }
        }

        // Delete event
        public async Task DeleteEventAsync(int eventId)
        {
            try
            {
                var ev = await _eventRepository.GetByIdAsync(eventId);
                if (ev == null)
                    throw new InvalidOperationException($"Event with Id {eventId} not found.");

                await _eventRepository.DeleteAsync(eventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, eventId);
                throw;
            }
        }

        // Get event with tickets
        public async Task<EventWithTicketsDto> GetEventWithTicketsAsync(int eventId)
        {
            try
            {
                var ev = await _eventRepository.GetEventWithTicketsAsync(eventId);
                if (ev == null)
                    throw new InvalidOperationException($"Event with Id {eventId} not found.");

                var venue = await _venueRepository.GetByIdAsync(ev.VenueId);
                if (venue == null)
                    throw new InvalidOperationException($"Venue with Id {ev.VenueId} not found.");

                return new EventWithTicketsDto
                {
                    Id = ev.Id,
                    Title = ev.Title,
                    StartTime = ev.StartTime,
                    EndTime = ev.EndTime,
                    Venue = new VenueResponseDto
                    {
                        Id = venue.Id,
                        Name = venue.Name,
                        Address = venue.Address,
                        Capacity = venue.Capacity
                    },
                    Tickets = ev.Tickets.Select(t => new TicketResponseDto
                    {
                        Id = t.Id,
                        SeatNumber = t.SeatNumber,
                        Price = t.Price,
                        PurchasedAt = t.PurchasedAt,
                        User = new UserResponseDto
                        {
                            Id = t.User.Id,
                            Name = t.User.Name,
                            Email = t.User.Email,
                            Role = t.User.Role
                        },
                        Event = MapEventToDto(ev, venue)
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, eventId);
                throw;
            }
        }

        // Get upcoming events
        public async Task<IEnumerable<EventResponseDto>> GetUpcomingEventsAsync()
        {
            try
            {
                var events = await _eventRepository.GetUpcomingEventsAsync();
                return await MapEventsToDto(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<EventPaginatedList> GetEventsPaginatedAsync(int page, int pageSize, string? venueName = null, string? sortBy = null)
        {
            try
            {
                // Get raw data from repository
                var (events, totalCount) = await _eventRepository.GetEventsPaginatedAsync(page, pageSize);

                // Apply filtering
                if (!string.IsNullOrWhiteSpace(venueName))
                {
                    events = events
                        .Where(e => e.Venue.Name.Contains(venueName, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    totalCount = events.Count();
                }

                // Apply sorting
                events = sortBy?.ToLower() switch
                {
                    "title" => events.OrderBy(e => e.Title).ToList(),
                    "starttime" => events.OrderBy(e => e.StartTime).ToList(),
                    "endtime" => events.OrderBy(e => e.EndTime).ToList(),
                    _ => events.OrderBy(e => e.StartTime).ToList()
                };

                // Map to DTOs
                var items = events.Select(e => new EventResponseDto
                {
                    Id = e.Id,
                    Title = e.Title,
                    StartTime = e.StartTime,
                    EndTime = e.EndTime,
                    Venue = new VenueResponseDto
                    {
                        Id = e.Venue.Id,
                        Name = e.Venue.Name,
                        Address = e.Venue.Address,
                        Capacity = e.Venue.Capacity
                    }
                }).ToList();

                return new EventPaginatedList(items, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }



        // Get most popular events
        public async Task<IEnumerable<EventResponseDto>> GetMostPopularEventsAsync(int topN)
        {
            try
            {
                var events = await _eventRepository.GetMostPopularEventsAsync(topN);
                return await MapEventsToDto(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMostPopularEventsAsync TopN {TopN}", topN);
                throw;
            }
        }

        // Helper: map Event to EventResponseDto
        private EventResponseDto MapEventToDto(Event ev, Venue venue)
        {
            return new EventResponseDto
            {
                Id = ev.Id,
                Title = ev.Title,
                StartTime = ev.StartTime,
                EndTime = ev.EndTime,
                Venue = new VenueResponseDto
                {
                    Id = venue.Id,
                    Name = venue.Name,
                    Address = venue.Address,
                    Capacity = venue.Capacity
                }
            };
        }

        // Helper: map Events to EventResponseDto
        private async Task<IEnumerable<EventResponseDto>> MapEventsToDto(IEnumerable<Event> events)
        {
            var list = new List<EventResponseDto>();
            foreach (var ev in events)
            {
                var venue = await _venueRepository.GetByIdAsync(ev.VenueId);
                if (venue == null)
                    throw new InvalidOperationException($"Venue with Id {ev.VenueId} not found.");

                list.Add(MapEventToDto(ev, venue));
            }
            return list;
        }
    }
}
