using Application.DTOs.UserDTOs;

namespace Application.DTOs.JWTDTOs
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
        public UserResponseDto User { get; set; } = null!;
    }
}
