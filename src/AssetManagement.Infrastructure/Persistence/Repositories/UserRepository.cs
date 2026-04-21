using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetManagement.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AssetManagementDbContext _context;

    public UserRepository(AssetManagementDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(
        int id, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<User>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .OrderBy(u => u.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User entity,
        CancellationToken cancellationToken = default)
    {
        await _context.Users.AddAsync(entity, cancellationToken);
    }

    public void Update(User entity)
    {
        _context.Users.Update(entity);
    }

    public void Remove(User entity)
    {
        _context.Users.Remove(entity);
    }

    public async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(
        string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(
        string email, CancellationToken cancellationToken = default)
    {
        return await _context.Users
            .AnyAsync(u => u.Email == email, cancellationToken);
    }
}