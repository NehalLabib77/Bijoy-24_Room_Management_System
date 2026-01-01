using ResidentialHallManagement.Core.Entities;

namespace ResidentialHallManagement.Core.Interfaces;

public interface IHallService
{
    Task<IEnumerable<Hall>> GetAllHallsAsync();
    Task<Hall?> GetHallByIdAsync(int hallId);
    Task<Hall> CreateHallAsync(Hall hall);
    Task<Hall> UpdateHallAsync(Hall hall);
    Task<bool> DeleteHallAsync(int hallId);
    Task<IEnumerable<Hall>> SearchHallsAsync(string? name = null, string? type = null);
}
