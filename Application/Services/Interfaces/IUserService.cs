using Application.DTOs.JWTDTOs;
using Application.DTOs.TicketDTO;
using Application.DTOs.UserDTOs;

namespace Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto);
        Task<AuthResponseDto> RegisterAsync(UserDto userDto);
        Task<UserResponseDto> GetByIdAsync(int userId);
        Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(string role);
        Task AssignTicketToUserAsync(int userId, TicketDto ticketDto);
        Task<UserResponseDto> CreateUserByAdminAsync(AdminCreateUserDto dto, int adminId);
    }
}
