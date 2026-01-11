using Application.DTOs.JWTDTOs;
using Application.DTOs.TicketDTO;
using Application.DTOs.UserDTOs;
using Application.Services.Interfaces;
using Application.Utilities;
using Domain.Models;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ITicketRepository _ticketRepository;
        private readonly ILogger<UserService> _logger;
        private readonly JwtSettings _jwtSettings;

        public UserService(
            IUserRepository userRepository,
            IEventRepository eventRepository,
            ITicketRepository ticketRepository,
            ILogger<UserService> logger,
            JwtSettings jwtSettings)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _ticketRepository = ticketRepository;
            _logger = logger;
            _jwtSettings = jwtSettings;
        }

        // Assign a ticket to a user with transaction safety
        public async Task AssignTicketToUserAsync(int userId, TicketDto ticketDto)
        {
            try
            {
                // Retrieve user
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {userId} not found.");
                }

                // Retrieve event
                var ev = await _eventRepository.GetByIdAsync(ticketDto.EventId);
                if (ev == null)
                {
                    throw new InvalidOperationException($"Event with ID {ticketDto.EventId} not found.");
                }

                // Check seat availability
                var seatAvailable = await _ticketRepository
                    .CheckSeatAvailabilityAsync(ticketDto.EventId, ticketDto.SeatNumber);
                if (!seatAvailable)
                {
                    throw new InvalidOperationException($"Seat {ticketDto.SeatNumber} is already taken.");
                }

                // Create ticket entity
                var ticket = new Ticket
                {
                    UserId = userId,
                    EventId = ticketDto.EventId,
                    SeatNumber = ticketDto.SeatNumber,
                    Price = ticketDto.Price,
                    PurchasedAt = DateTime.UtcNow
                };

                // Purchase ticket (save to DB)
                await _ticketRepository.PurchaseTicketAsync(ticket);
            }
            catch (Exception ex)
            {
                // Log unexpected errors
                _logger.LogError(ex, ex.Message,
                    userId, ticketDto.EventId, ticketDto.SeatNumber);
                throw;
            }
        }

        // Get user by ID
        public async Task<UserResponseDto> GetByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new InvalidOperationException($"User with ID {userId} not found.");
                }

                return new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, userId);
                throw;
            }
        }

        // Get users filtered by role
        public async Task<IEnumerable<UserResponseDto>> GetUsersByRoleAsync(string role)
        {
            try
            {
                return await _userRepository.GetUsersByRoleAsync(role).Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role
                }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, role);
                throw;
            }
        }

        // User login
        public async Task<AuthResponseDto> RegisterAsync(UserDto userDto)
        {
            try
            {
                var existingUser = await _userRepository.GetByEmailAsync(userDto.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Email is already registered.");
                }

                var user = new User
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Role = Roles.Attendee,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
                };

                await _userRepository.AddAsync(user);

                var token = JwtTokenGenerator.GenerateToken(user, _jwtSettings);

                return new AuthResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                    User = new UserResponseDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = user.Role
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, userDto.Email);
                throw;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(UserLoginDto loginDto)
        {
            try
            {
                var user = await _userRepository.GetByEmailAsync(loginDto.Email);
                if (user == null)
                {
                    throw new InvalidOperationException($"User with email {loginDto.Email} not found.");
                }

                bool valid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
                if (!valid)
                {
                    throw new InvalidOperationException("Invalid login credentials.");
                }

                var token = JwtTokenGenerator.GenerateToken(user, _jwtSettings);

                return new AuthResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                    User = new UserResponseDto
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Role = user.Role
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, loginDto.Email);
                throw;
            }
        }

        public async Task<UserResponseDto> CreateUserByAdminAsync(AdminCreateUserDto dto, int adminId)
        {
            try
            {
                var admin = await _userRepository.GetByIdAsync(adminId);
                if (admin == null || admin.Role != Roles.Admin)
                    throw new UnauthorizedAccessException("Only admins can create users.");

                var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
                if (existingUser != null)
                    throw new InvalidOperationException("Email is already registered.");

                var user = new User
                {
                    Name = dto.Name,
                    Email = dto.Email,
                    Role = dto.Role,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
                };

                await _userRepository.AddAsync(user);

                return new UserResponseDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

    }
}
