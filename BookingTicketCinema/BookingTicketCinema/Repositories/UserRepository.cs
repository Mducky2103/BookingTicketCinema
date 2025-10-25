using System;
using BookingTicketCinema.Data;
using BookingTicketCinema.Models;
using BookingTicketCinema.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookingTicketCinema.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly CinemaDbContext _context;

        public UserRepository(CinemaDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(User user) =>
            await _context.Users.AddAsync(user);

        public async Task UpdateAsync(User user) =>
        _context.Users.Update(user);

        public async Task DeleteAsync(User user) =>
            _context.Users.Remove(user);

        public async Task SaveChangesAsync() =>
            await _context.SaveChangesAsync();

        public async Task<IEnumerable<User>> GetAllAsync() =>
        await _context.Users.ToListAsync();

        public async Task<User?> GetByIdAsync(string id) =>
            await _context.Users.FindAsync(id);

        public async Task<User?> GetByEmailAsync(string email) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
