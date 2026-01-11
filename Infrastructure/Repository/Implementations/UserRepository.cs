using Domain.Models;
using Infrastructure.Data;
using Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {

        }
        public async Task AssignTicketToUserAsync(int userId, Ticket ticket)
        {
            var user = await _dbSet
                .Include(u => u.Tickets)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                throw new InvalidOperationException($"User with ID {userId} not found.");
            }

            user.Tickets.Add(ticket);

            await _context.SaveChangesAsync();
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            return _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public IQueryable<User> GetUsersByRoleAsync(string role)
        {
            return _dbSet
                .Where(u => u.Role == role);
        }
    }
}
