using Application.DTOs.VenueDTOs;
using Application.Services.Interfaces;
using Application.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VenuesController : ControllerBase
    {
        private readonly IVenueService _venueService;
        public VenuesController(IVenueService venueService)
        {
            _venueService = venueService;
        }

        // GET api/venues/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<VenueWithEventsDto>> GetWithEvents(int id)
        {
            var venue = await _venueService.GetVenueWithEventsAsync(id);
            return Ok(venue);
        }

        // GET api/venues/available
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<VenueResponseDto>>> GetAvailable([FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
        {
            var venues = await _venueService.GetAvailableVenuesAsync(startTime, endTime);
            return Ok(venues);
        }

        // POST api/venues
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<ActionResult<VenueResponseDto>> Create([FromBody] VenueDto dto)
        {
            var venue = await _venueService.CreateVenueAsync(dto);
            return CreatedAtAction(nameof(GetWithEvents), new { id = venue.Id }, venue);
        }

        // PUT api/venues/{id}
        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<VenueResponseDto>> Update(int id, [FromBody] VenueDto dto)
        {
            var venue = await _venueService.UpdateVenueAsync(id, dto);
            return Ok(venue);
        }

        // DELETE api/venues/{id}
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _venueService.DeleteVenueAsync(id);
            return NoContent();
        }

        // GET api/venues/{id}/capacity
        [HttpGet("{id}/capacity")]
        public async Task<ActionResult<int>> GetCapacity(int id)
        {
            var capacity = await _venueService.CheckCapacityAsync(id);
            return Ok(capacity);
        }
    }
}
