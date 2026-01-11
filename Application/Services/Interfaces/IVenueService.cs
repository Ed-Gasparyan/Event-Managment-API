using Application.DTOs.VenueDTOs;

namespace Application.Services.Interfaces
{
    public interface IVenueService
    {
        Task<VenueResponseDto> CreateVenueAsync(VenueDto venueDto);
        Task<VenueResponseDto> UpdateVenueAsync(int venueId, VenueDto venueDto);
        Task DeleteVenueAsync(int venueId);
        Task<VenueWithEventsDto> GetVenueWithEventsAsync(int venueId);
        Task<int> CheckCapacityAsync(int venueId);
        Task<IEnumerable<VenueResponseDto>> GetAvailableVenuesAsync(DateTime startTime, DateTime endTime);
    }
}
