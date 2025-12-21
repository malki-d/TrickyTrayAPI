using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Repositories
{
    public interface IDonorRepository
    {
        Task<Donor> AddDonor(Donor donor);
        Task<bool> DeleteDonor(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<Donor>> GetAllDonors();
        Task<Donor?> GetDonorById(int id);
        Task<Donor> UpdateAsync(Donor donor);
    }
}