using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Repositories
{
    public interface IDonorRepository
    {
        Task<Donor> AddDonor(CreateDonorDTO donor);
        Task<bool> DeleteDonor(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Donor>> GetAllDonors();
        Task<Donor?> GetDonorById(int id);
        Task<Donor> UpdateAsync(int id, CreateDonorDTO donor);

        Task<Donor?> GetDonorWithGiftsByIdAsync(int donorId);
        Task<IEnumerable<Donor>> FilterDonorsAsync(string? name, string? email, string? giftName);
    }
}