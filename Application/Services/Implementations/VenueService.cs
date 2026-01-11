using Application.DTOs.VenueDTOs;
using Application.DTOs.EventDTOs;
using Application.Services.Interfaces;
using Domain.Models;
using Infrastructure.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
    public class VenueService : IVenueService
    {
        private readonly IVenueRepository _venueRepository;
        private readonly ILogger<VenueService> _logger;

        public VenueService(IVenueRepository venueRepository, ILogger<VenueService> logger)
        {
            _venueRepository = venueRepository;
            _logger = logger;
        }

        // Check venue capacity
        public async Task<int> CheckCapacityAsync(int venueId)
        {
            try
            {
                var capacity = await _venueRepository.CheckCapacityAsync(venueId);

                if (capacity == 0)
                    throw new InvalidOperationException($"Venue with Id {venueId} not found.");

                return capacity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, venueId);
                throw;
            }
        }

        // Create a new venue
        public async Task<VenueResponseDto> CreateVenueAsync(VenueDto venueDto)
        {
            try
            {
                var venue = new Venue
                {
                    Name = venueDto.Name,
                    Address = venueDto.Address,
                    Capacity = venueDto.Capacity
                };

                await _venueRepository.AddAsync(venue);
                return new VenueResponseDto
                {
                    Id = venue.Id,
                    Name = venue.Name,
                    Address = venue.Address,
                    Capacity = venue.Capacity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, venueDto);
                throw;
            }
        }

        // Delete a venue
        public async Task DeleteVenueAsync(int venueId)
        {
            try
            {
                var venue = await _venueRepository.GetByIdAsync(venueId);
                if (venue == null)
                    throw new InvalidOperationException($"Venue with Id {venueId} not found.");

                await _venueRepository.DeleteAsync(venueId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, venueId);
                throw;
            }
        }

        // Get available venues in a time range
        public async Task<IEnumerable<VenueResponseDto>> GetAvailableVenuesAsync(DateTime startTime, DateTime endTime)
        {
            try
            {
                var venues = await _venueRepository.GetAvailableVenuesAsync(startTime, endTime);

                if (!venues.Any())
                    throw new InvalidOperationException("No available venues found for the given time range.");

                return venues.Select(v => new VenueResponseDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    Capacity = v.Capacity
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, startTime, endTime);
                throw;
            }
        }

        // Get a venue with its events
        public async Task<VenueWithEventsDto> GetVenueWithEventsAsync(int venueId)
        {
            try
            {
                var venue = await _venueRepository.GetVenueWithEventsAsync(venueId);
                if (venue == null)
                    throw new InvalidOperationException($"Venue with Id {venueId} not found.");

                return new VenueWithEventsDto
                {
                    Id = venue.Id,
                    Name = venue.Name,
                    Address = venue.Address,
                    Capacity = venue.Capacity,
                    Events = venue.Events.Select(e => new EventResponseDto
                    {
                        Id = e.Id,
                        Title = e.Title,
                        StartTime = e.StartTime,
                        EndTime = e.EndTime,
                        Venue = new VenueResponseDto
                        {
                            Id = venue.Id,
                            Name = venue.Name,
                            Address = venue.Address,
                            Capacity = venue.Capacity
                        }
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, venueId);
                throw;
            }
        }


        // Update an existing venue
        public async Task<VenueResponseDto> UpdateVenueAsync(int venueId, VenueDto venueDto)
        {
            try
            {
                var venue = await _venueRepository.GetByIdAsync(venueId);
                if (venue == null)
                    throw new InvalidOperationException($"Venue with Id {venueId} not found.");

                venue.Name = venueDto.Name;
                venue.Address = venueDto.Address;
                venue.Capacity = venueDto.Capacity;

                await _venueRepository.UpdateAsync(venue);

                return new VenueResponseDto
                {
                    Id = venue.Id,
                    Name = venue.Name,
                    Address = venue.Address,
                    Capacity = venue.Capacity
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, venueId, venueDto);
                throw;
            }
        }
    }
}
