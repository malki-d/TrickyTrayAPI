using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.Models;
using WebApi.Data;

namespace TrickyTrayAPI.Repositories
{
    public class GiftRepository : IGiftRepository
    {
        private readonly AppDbContext _context;

        public GiftRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Gift>> GetAllAsync()
        {
            return await _context.Gifts
                .Include(p => p.Category.Name)
                .ToListAsync();
        }

        public async Task<Gift?> GetByIdAsync(int id)
        {
            return await _context.Gifts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Gift> AddAsync(Gift gift)
        {
            _context.Gifts.Add(gift);
            await _context.SaveChangesAsync();
            return gift;
        }

        public async Task<Gift> UpdateAsync(Gift gift)
        {
            _context.Gifts.Update(gift);
            await _context.SaveChangesAsync();
            return gift;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Gifts.FindAsync(id);
            if (product == null)
                return false;

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
