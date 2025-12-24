using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Services;
using WebApi.Data;

namespace TrickyTrayAPI.Repositories
{
    public class GiftRepository : IGiftRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GiftService> _logger;

        public GiftRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Gift>> GetAllAsync()
        {
            return await _context.Gifts.Include(x=>x.Category).Include(x => x.Donor).Include(x => x.Winner).ToListAsync();
        }

        public async Task<Gift?> GetByIdAsync(int id)
        {
            return await _context.Gifts.Include(x=>x.Category).Include(x => x.Donor).Include(x => x.Winner).FirstOrDefaultAsync(p => p.Id == id);
        }
       
        public async Task<Gift> AddAsync(CreateGiftDTO gift)
        {

            var g = new Gift { Name = gift.Name, Description = gift.Description, DonorId = gift.DonorId, CategoryId = gift.CategoryId };
            _context.Gifts.Add(g);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(g.Id);
        }

        public async Task<Gift> UpdateAsync(UpdateGiftDTO gift, int id)
        {

            var g = await GetByIdAsync(id);
            g.Name = gift.Name;
            g.CategoryId = gift.CategoryId;
            g.Description = gift.Description;
          
            await _context.SaveChangesAsync();
            return await GetByIdAsync(g.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Gifts.FindAsync(id);
            if (product == null || product.Users.Count > 0)
            {
                _logger.LogInformation("users buy thus gift cand delete " + id);
                return false;

            }
            if (product == null )
            {
                _logger.LogInformation("cand find product " + id);
                return false;
            }

            _context.Gifts.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Gifts.AnyAsync(p => p.Id == id);
        }


    }
}
