using Application.DTOs.JWTDTOs;
using Application.DTOs.TicketDTO;
using Application.DTOs.UserDTOs;
using Application.Services.Interfaces;
using Application.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // GET api/users/{id}
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserResponseDto>> GetById(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            return Ok(user);
        }

        // GET api/users/role/{role}
        [Authorize(Roles = Roles.Admin)]
        [HttpGet("role/{role}")]
        public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUsersByRole(string role)
        {
            var users = await _userService.GetUsersByRoleAsync(role);
            return Ok(users);
        }

        // POST api/users/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] UserDto dto)
        {
            // Register user and generate JWT
            var authResponse = await _userService.RegisterAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = authResponse.User.Id }, authResponse);
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] UserLoginDto dto)
        {
            var token = await _userService.LoginAsync(dto); // updated service method
            return Ok(token);
        }

        // POST api/users/{id}/tickets
        [Authorize]
        [HttpPost("{id}/tickets")]
        public async Task<IActionResult> AssignTicket(int id, [FromBody] TicketDto ticketDto)
        {
            await _userService.AssignTicketToUserAsync(id, ticketDto);
            return NoContent();
        }

        // POST api/users/create
        [Authorize(Roles = Roles.Admin)]
        [HttpPost("admin/create-user")]
        public async Task<ActionResult<UserResponseDto>> CreateUser([FromBody] AdminCreateUserDto dto)
        {
            var adminId = int.Parse(User.FindFirst("id")!.Value);

            var user = await _userService.CreateUserByAdminAsync(dto, adminId);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

    }
}
