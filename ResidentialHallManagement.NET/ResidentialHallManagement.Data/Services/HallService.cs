using Microsoft.EntityFrameworkCore;
using ResidentialHallManagement.Core.Entities;
using ResidentialHallManagement.Core.Interfaces;
using ResidentialHallManagement.Data;

namespace ResidentialHallManagement.Data.Services;

public class HallService : IHallService
{
    private readonly HallManagementDbContext _context;

    public HallService(HallManagementDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Hall>> GetAllHallsAsync()
    {
        return await _context.Halls
            .Include(h => h.Rooms)
            .ToListAsync();
    }

    public async Task<Hall?> GetHallByIdAsync(int hallId)
    {
        return await _context.Halls
            .Include(h => h.Rooms)
            .FirstOrDefaultAsync(h => h.HallId == hallId);
    }

    public async Task<Hall> CreateHallAsync(Hall hall)
    {
        _context.Halls.Add(hall);
        await _context.SaveChangesAsync();
        return hall;
    }

    public async Task<Hall> UpdateHallAsync(Hall hall)
    {
        _context.Entry(hall).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return hall;
    }

    public async Task<bool> DeleteHallAsync(int hallId)
    {
        var hall = await _context.Halls.FindAsync(hallId);
        if (hall == null) return false;

        _context.Halls.Remove(hall);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Hall>> SearchHallsAsync(string? name = null, string? type = null)
    {
        var query = _context.Halls.AsQueryable();

        if (!string.IsNullOrEmpty(name))
        {
            query = query.Where(h => h.HallName.Contains(name));
        }

        if (!string.IsNullOrEmpty(type))
        {
            query = query.Where(h => h.HallType == type.ToUpper());
        }

        return await query.ToListAsync();
    }
}
