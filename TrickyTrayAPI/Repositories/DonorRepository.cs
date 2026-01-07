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

        public async Task<Donor> UpdateAsync(int id, CreateDonorDTO donor)
        {
            var donorex= await _context.Donors.FirstOrDefaultAsync(p => p.Id == id);
            donorex.Email=donor.Email;
            donorex.PhoneNumber=donor.PhoneNumber;
            donor.Name=donor.Name;
            await _context.SaveChangesAsync();
            return donorex;
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

        public async Task<Donor?> GetDonorWithGiftsByIdAsync(int donorId)
        {
            return await _context.Donors
                .Include(d => d.Gifts)
                .ThenInclude(g => g.Category)
                .FirstOrDefaultAsync(d => d.Id == donorId);
        }
        public async Task<IEnumerable<Donor>> FilterDonorsAsync(string? name, string? email, string? giftName)
        {
            var query = _context.Donors.AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(d => d.Name.Contains(name));

            if (!string.IsNullOrEmpty(email))
                query = query.Where(d => d.Email.Contains(email));

            if (!string.IsNullOrEmpty(giftName))
                query = query.Where(d => d.Gifts.Any(g => g.Name.Contains(giftName)));

            return await query.Include(d => d.Gifts).ThenInclude(g => g.Category).ToListAsync();
        }



    }
}
