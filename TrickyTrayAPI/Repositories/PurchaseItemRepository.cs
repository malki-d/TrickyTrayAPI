using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.Models;
using WebApi.Data;

namespace TrickyTrayAPI.Repositories
{
    public class PurchaseItemRepository : IPurchaseItemRepository
    {


        private readonly AppDbContext _context;

        public PurchaseItemRepository(AppDbContext context)
        {
            _context = context;
        }



        public async Task<IEnumerable<PurchaseItem>> GetPurchaseItemsForGiftAsync(int giftId)
        {
            return await _context.PurchaseItems
                .Where(pi => pi.GiftId == giftId)
                .Include(pi => pi.User)
                .Include(pi=>pi.Gift)
                .ToListAsync();
        }
    }
}
