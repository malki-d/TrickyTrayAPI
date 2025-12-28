using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.Models; // Adjust namespace as needed
using WebApi.Data; // Adjust if your AppDbContext is in a different namespace
using TrickyTrayAPI.DTOs;

namespace TrickyTrayAPI.Repositories
{
    public class PurchaseRepository : IPurchaseRepository
    {
        private readonly AppDbContext _context;

        public PurchaseRepository(AppDbContext context)
        {
            _context = context;
        }

        // Get all purchases
        public async Task<IEnumerable<Purchase>> GetAllAsync()
        {
            return await _context.Purchases.ToListAsync();
        }

        // Get purchase by id
        public async Task<Purchase?> GetByIdAsync(int id)
        {
            return await _context.Purchases.FindAsync(id);
        }

        // Add new purchase
        public async Task<Purchase> AddAsync(Purchase purchase)
        {
            _context.Purchases.Add(purchase);
            await _context.SaveChangesAsync();
            return purchase;
        }

        // Update purchase
        public async Task<bool> UpdateAsync(Purchase purchase)
        {
            var existing = await _context.Purchases.FindAsync(purchase.Id);
            if (existing == null)
                return false;

            // Update properties
            existing.Date = purchase.Date;
            existing.UserId = purchase.UserId;
            existing.Price = purchase.Price;
            // Add other properties as needed

            await _context.SaveChangesAsync();
            return true;
        }

        // Delete purchase
        public async Task<bool> DeleteAsync(int id)
        {
            var purchase = await _context.Purchases.FindAsync(id);
            if (purchase == null)
                return false;

            _context.Purchases.Remove(purchase);
            await _context.SaveChangesAsync();
            return true;
        }

        // Get total revenue
        public async Task<PurchaseRevenueDTO> GetTotalRevenueAsync()
        {
            var total = await _context.Purchases.SumAsync(p => (int?)p.Price) ?? 0;
            return new PurchaseRevenueDTO { TotalRevenue = total, AsOf = DateTime.UtcNow };
        }
    }


}