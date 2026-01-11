using Application.DTOs.TicketDTO;
using Application.DTOs.UserDTOs;
using Application.DTOs.EventDTOs;
using Application.Services.Interfaces;
using Infrastructure.Repository.Interfaces;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<TicketService> _logger;

        public TicketService(ITicketRepository ticketRepository, ILogger<TicketService> logger)
        {
            _ticketRepository = ticketRepository;
            _logger = logger;
        }

        // Check if a seat is available for a specific event
        public async Task<bool> CheckSeatAvailabilityAsync(int eventId, string seatNumber)
        {
            try
            {
                // Ask repository whether the seat is free
                return await _ticketRepository.CheckSeatAvailabilityAsync(eventId, seatNumber);
            }
            catch (Exception ex)
            {
                // Any other unexpected error
                _logger.LogError(ex, ex.Message, eventId, seatNumber);
                throw;
            }
        }

        // Calculate total revenue for a specific event
        public async Task<decimal> GetRevenuePerEventAsync(int eventId)
        {
            try
            {
                // Ask repository to sum all ticket prices for the event
                return await _ticketRepository.GetRevenuePerEventAsync(eventId);
            }
            catch (Exception ex)
            {
                // Unexpected error
                _logger.LogError(ex, ex.Message, eventId);
                throw;
            }
        }

        // Get all tickets for a specific event
        public async Task<IEnumerable<TicketResponseDto>> GetTicketsByEventAsync(int eventId)
        {
            try
            {
                // Retrieve tickets from repository
                var tickets = await _ticketRepository.GetTicketsByEventAsync(eventId);

                // Map ticket entities to response DTOs including User and Event info
                return tickets.Select(t => new TicketResponseDto
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
                    Event = new EventResponseDto
                    {
                        Id = t.Event.Id,
                        Title = t.Event.Title,
                        StartTime = t.Event.StartTime,
                        EndTime = t.Event.EndTime,
                        Venue = new Application.DTOs.VenueDTOs.VenueResponseDto
                        {
                            Id = t.Event.Venue.Id,
                            Name = t.Event.Venue.Name,
                            Address = t.Event.Venue.Address,
                            Capacity = t.Event.Venue.Capacity
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                // Any other unexpected error
                _logger.LogError(ex, ex.Message, eventId);
                throw;
            }
        }

        // Get all tickets purchased by a specific user
        public async Task<IEnumerable<TicketResponseDto>> GetTicketsByUserAsync(int userId)
        {
            try
            {
                // Retrieve tickets for the user
                var tickets = await _ticketRepository.GetTicketsByUserAsync(userId);

                // Map ticket entities to response DTOs including User and Event info
                return tickets.Select(t => new TicketResponseDto
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
                    Event = new EventResponseDto
                    {
                        Id = t.Event.Id,
                        Title = t.Event.Title,
                        StartTime = t.Event.StartTime,
                        EndTime = t.Event.EndTime,
                        Venue = new Application.DTOs.VenueDTOs.VenueResponseDto
                        {
                            Id = t.Event.Venue.Id,
                            Name = t.Event.Venue.Name,
                            Address = t.Event.Venue.Address,
                            Capacity = t.Event.Venue.Capacity
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                // Unexpected error
                _logger.LogError(ex, ex.Message, userId);
                throw;
            }
        }

        // Get total number of tickets sold for an event
        public async Task<int> GetTotalTicketsSoldAsync(int eventId)
        {
            try
            {
                // Ask repository to count tickets
                return await _ticketRepository.GetTotalTicketsSoldAsync(eventId);
            }
            catch (Exception ex)
            {
                // Any other unexpected error
                _logger.LogError(ex, ex.Message, eventId);
                throw;
            }
        }

        // Purchase a ticket and return its response DTO
        public async Task<TicketResponseDto> PurchaseTicketAsync(TicketDto ticketDto)
        {
            try
            {
                // Ensure seat is available before purchase
                bool seatAvailable = await _ticketRepository.CheckSeatAvailabilityAsync(ticketDto.EventId, ticketDto.SeatNumber);
                if (!seatAvailable)
                {
                    throw new InvalidOperationException($"Seat {ticketDto.SeatNumber} is already taken for EventId {ticketDto.EventId}");
                }

                // Map DTO to Ticket entity
                var ticket = new Ticket
                {
                    EventId = ticketDto.EventId,
                    UserId = ticketDto.UserId,
                    SeatNumber = ticketDto.SeatNumber,
                    Price = ticketDto.Price,
                    PurchasedAt = DateTime.UtcNow
                };

                // Save ticket in database
                await _ticketRepository.PurchaseTicketAsync(ticket);

                // Map ticket entity to response DTO including User and Event info
                return new TicketResponseDto
                {
                    Id = ticket.Id,
                    SeatNumber = ticket.SeatNumber,
                    Price = ticket.Price,
                    PurchasedAt = ticket.PurchasedAt,
                    User = new UserResponseDto
                    {
                        Id = ticket.User.Id,
                        Name = ticket.User.Name,
                        Email = ticket.User.Email,
                        Role = ticket.User.Role
                    },
                    Event = new EventResponseDto
                    {
                        Id = ticket.Event.Id,
                        Title = ticket.Event.Title,
                        StartTime = ticket.Event.StartTime,
                        EndTime = ticket.Event.EndTime,
                        Venue = new Application.DTOs.VenueDTOs.VenueResponseDto
                        {
                            Id = ticket.Event.Venue.Id,
                            Name = ticket.Event.Venue.Name,
                            Address = ticket.Event.Venue.Address,
                            Capacity = ticket.Event.Venue.Capacity
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                // Unexpected error
                _logger.LogError(ex, ex.Message,
                    ticketDto.UserId, ticketDto.EventId, ticketDto.SeatNumber);
                throw;
            }
        }
    }
}
