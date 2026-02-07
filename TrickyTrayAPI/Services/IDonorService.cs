using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.Services
{
    public interface IDonorService
    {
        Task<Donor> AddDonor(CreateDonorDTO donor);
        Task<bool> DeleteDonor(int id);
        Task<bool> ExistsAsync(int id);
        Task<IEnumerable<GetDonorWithGiftsDTO>> GetAllDonors();
        Task<GetDonorDTO?> GetDonorById(int id);
        Task<GetDonorDTO> UpdateAsync(int id, CreateDonorDTO donor);
        Task<GetDonorWithGiftsDTO?> GetDonorWithGiftsAsync(int donorId);
        Task<IEnumerable<GetDonorWithGiftsDTO>> FilterDonorsAsync(string? name, string? email, string? giftName);
    }
}