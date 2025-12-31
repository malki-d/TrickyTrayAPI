using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using WebApi.Data;
using Microsoft.Extensions.Logging;

namespace TrickyTrayAPI.Repositories
{
    public class PurchaseItemRepository : IPurchaseItemRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PurchaseItemRepository> _logger;

        public PurchaseItemRepository(AppDbContext context, ILogger<PurchaseItemRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<PurchaseItem>> GetAllAsync()
        {
            return await _context.PurchaseItems
                .Include(pi => pi.Gift)
                .Include(pi => pi.User)
                .ToListAsync();
        }

        public async Task<PurchaseItem?> GetByIdAsync(int id)
        {
            return await _context.PurchaseItems
                .Include(pi => pi.Gift)
                .Include(pi => pi.User)
                .FirstOrDefaultAsync(pi => pi.Id == id);
        }

        public async Task<PurchaseItem> AddAsync(CreatePurchaseItemDTO purchaseItem)
        {
            var pi = new PurchaseItem
            {
                GiftId = purchaseItem.GiftId,
                UserId = purchaseItem.UserId,
                IsWinner = false
            };

            _context.PurchaseItems.Add(pi);
            await _context.SaveChangesAsync();
            return await GetByIdAsync(pi.Id);
        }

        public async Task<PurchaseItem> UpdateAsync(UpdatePurchaseItemDTO purchaseItem, int id)
        {
            var pi = await GetByIdAsync(id);
            if (pi == null)
            {
                _logger.LogWarning("PurchaseItem not found for update: {Id}", id);
                return null;
            }

            pi.GiftId = purchaseItem.GiftId;
            pi.UserId = purchaseItem.UserId;
            pi.IsWinner = purchaseItem.IsWinner;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(pi.Id);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var purchaseItem = await _context.PurchaseItems.FindAsync(id);
            if (purchaseItem == null)
            {
                _logger.LogWarning("Cannot find PurchaseItem to delete: {Id}", id);
                return false;
            }

            _context.PurchaseItems.Remove(purchaseItem);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.PurchaseItems.AnyAsync(pi => pi.Id == id);
        }

        public async Task<IEnumerable<PurchaseItem>> GetPurchaseItemsForGiftAsync(int giftId)
        {
            return await _context.PurchaseItems
                .Where(pi => pi.GiftId == giftId)
                .Include(pi => pi.User)
                .Include(pi => pi.Gift)
                .ToListAsync();
        }

        public async Task<IEnumerable<PurchaseItem>> GetPurchaseItemsForUserAsync(int userId)
        {
            return await _context.PurchaseItems
                .Where(pi => pi.UserId == userId)
                .Include(pi => pi.Gift)
                .Include(pi => pi.User)
                .ToListAsync();
        }
    }
}
