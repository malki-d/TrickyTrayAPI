using Microsoft.EntityFrameworkCore;
using System.Drawing;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Repositories;
using WebApi.Data;

namespace TrickyTrayAPI.Services
{
    public class DonorService : IDonorService
    {
        private readonly IDonorRepository _donorrepository;

        public DonorService(IDonorRepository donorrepository)
        {
            _donorrepository = donorrepository;
        }

        public async Task<IEnumerable<Donor>> GetAllDonors()
        {
            return await _donorrepository.GetAllDonors();
        }

        public async Task<Donor?> GetDonorById(int id)
        {
            return await _donorrepository.GetDonorById(id);

        }

        public async Task<Donor> AddDonor(Donor donor)
        {
            return await _donorrepository.AddDonor(donor);

        }

        public async Task<Donor> UpdateAsync(Donor donor)
        {
            return await _donorrepository.UpdateAsync(donor);

        }

        public async Task<bool> DeleteDonor(int id)
        {
            return await _donorrepository.DeleteDonor(id);

        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _donorrepository.ExistsAsync(id);
        }
    }
}
