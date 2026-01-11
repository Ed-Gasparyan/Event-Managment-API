using Application.DTOs.TicketDTO;
using Application.Services.Interfaces;
using Application.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        // POST api/tickets/purchase
        [HttpPost("purchase")]
        public async Task<ActionResult<TicketResponseDto>> Purchase([FromBody] TicketDto dto)
        {
            var ticket = await _ticketService.PurchaseTicketAsync(dto);
            return CreatedAtAction(nameof(GetTicketsByUser), new { userId = dto.UserId }, ticket);
        }

        // GET api/tickets/event/{eventId}
        [HttpGet("event/{eventId}")]
        public async Task<ActionResult<IEnumerable<TicketResponseDto>>> GetByEvent(int eventId)
        {
            var tickets = await _ticketService.GetTicketsByEventAsync(eventId);
            return Ok(tickets);
        }

        // GET api/tickets/user/{userId}
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<TicketResponseDto>>> GetTicketsByUser(int userId)
        {
            var tickets = await _ticketService.GetTicketsByUserAsync(userId);
            return Ok(tickets);
        }

        // GET api/tickets/event/{eventId}/revenue
        [Authorize(Roles = Roles.Admin)]
        [HttpGet("event/{eventId}/revenue")]
        public async Task<ActionResult<decimal>> GetRevenue(int eventId)
        {
            var revenue = await _ticketService.GetRevenuePerEventAsync(eventId);
            return Ok(revenue);
        }
    }
}
