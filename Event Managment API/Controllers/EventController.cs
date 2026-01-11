using Application.DTOs.EventDTOs;
using Application.Services.Interfaces;
using Application.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        // GET api/events/upcoming
        [HttpGet("upcoming")]
        public async Task<ActionResult<IEnumerable<EventResponseDto>>> GetUpcoming()
        {
            var events = await _eventService.GetUpcomingEventsAsync();
            return Ok(events);
        }

        // GET api/events/{id}/tickets
        [HttpGet("{id}/tickets")]
        public async Task<ActionResult<EventWithTicketsDto>> GetEventWithTickets(int id)
        {
            var ev = await _eventService.GetEventWithTicketsAsync(id);
            return Ok(ev);
        }

        // GET api/events
        [HttpGet]
        public async Task<ActionResult<EventPaginatedList>> GetPaginated([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? venueName = null, [FromQuery] string? sortBy = null)
        {
            var paginatedEvents = await _eventService.GetEventsPaginatedAsync(page, pageSize, venueName, sortBy);
            return Ok(paginatedEvents);
        }

        // POST api/events
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> Create([FromBody] EventDto dto)
        {
            var ev = await _eventService.CreateEventAsync(dto);
            return CreatedAtAction(nameof(GetEventWithTickets), new { id = ev.Id }, ev);
        }

        // PUT api/events/{id}
        [HttpPut("{id}")]
        [Authorize(Roles = Roles.Admin)]
        public async Task<ActionResult<EventResponseDto>> Update(int id, [FromBody] EventDto dto)
        {
            var ev = await _eventService.UpdateEventAsync(id, dto);
            return Ok(ev);
        }

        // DELETE api/events/{id}
        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _eventService.DeleteEventAsync(id);
            return NoContent();
        }
    }
}
