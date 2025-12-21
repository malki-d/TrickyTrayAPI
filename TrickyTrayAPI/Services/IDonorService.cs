using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Services
{
    public interface IDonorService
    {
        Task<Donor> AddDonor(Donor donor);
        Task<bool> DeleteDonor(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Donor>> GetAllDonors();
        Task<Donor?> GetDonorById(int id);
        Task<Donor> UpdateAsync(Donor donor);
    }
}