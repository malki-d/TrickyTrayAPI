using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using WebApi.Data;


public class PurchaseRepository : IPurchaseRepository
{
    private readonly AppDbContext _context;

    public PurchaseRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Purchase>> GetAllAsync()
    {
        return await _context.Purchases.Include(x => x.User).ToListAsync();
    }

    public async Task<IEnumerable<Purchase>> GetAllAsyncByUserId(int userId)
    {
        return await _context.Purchases
            .Where(x => x.UserId == userId)
            .Include(x => x.PurchaseItems)      // טוען את רשימת הפריטים
                .ThenInclude(pi => pi.Gift)     // טוען את פרטי המתנה לכל פריט!
            .ToListAsync();
    }
    public async Task<PurchaseRevenueDTO> GetTotalRevenueAsync()
    {
        var total = await _context.Purchases.SumAsync(p => (int?)p.Price) ?? 0;
        return new PurchaseRevenueDTO { TotalRevenue = total, AsOf = DateTime.UtcNow };
    }
    public async Task<List<CartItem>> GetCartItemsByUserIdAsync(int userId)
    {
        return await _context.CartItems
                             .Include(c => c.Gift) // חשוב כדי לדעת איזה מתנה זו
                             .Where(c => c.UserId == userId)
                             .ToListAsync();
    }

    public async Task AddPurchaseAsync(Purchase purchase)
    {
        await _context.Purchases.AddAsync(purchase);
    }

    public async Task ClearUserCartAsync(int userId)
    {
        // מחיקת כל הפריטים בעגלה של המשתמש הספציפי
        var itemsToRemove = await _context.CartItems
                                          .Where(c => c.UserId == userId)
                                          .ToListAsync();

        if (itemsToRemove.Any())
        {
            _context.CartItems.RemoveRange(itemsToRemove);
        }
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}