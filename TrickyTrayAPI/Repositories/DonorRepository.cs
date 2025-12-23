using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using WebApi.Data;

namespace TrickyTrayAPI.Repositories
{
    public class DonorRepository : IDonorRepository
    {
        private readonly AppDbContext _context;

        public DonorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Donor>> GetAllDonors()
        {
            return await _context.Donors.ToListAsync();
        }

        public async Task<Donor?> GetDonorById(int id)
        {
            return await _context.Donors
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Donor> AddDonor(CreateDonorDTO donor)
        {
            var item = new Donor { Name = donor.Name, Email = donor.Email, PhoneNumber = donor.PhoneNumber };
            _context.Donors.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<Donor> UpdateAsync(Donor donor)
        {
            _context.Donors.Update(donor);
            await _context.SaveChangesAsync();
            return donor;
        }

        public async Task<bool> DeleteDonor(int id)
        {
            var donor = await _context.Donors.FindAsync(id);
            if (donor == null)
                return false;

            _context.Donors.Remove(donor);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Donors.AnyAsync(p => p.Id == id);
        }


    }
}
