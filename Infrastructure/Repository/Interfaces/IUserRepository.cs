using Domain.Models;

namespace Infrastructure.Repository.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        IQueryable<User> GetUsersByRoleAsync(string role);
        Task AssignTicketToUserAsync(int userId, Ticket ticket);
    }
}
