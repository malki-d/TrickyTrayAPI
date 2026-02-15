using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Services;
using WebApi.Data;
using System.Linq;

namespace TrickyTrayAPI.Repositories
{
    public class GiftRepository : IGiftRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GiftRepository> _logger;

        public GiftRepository(AppDbContext context, ILogger<GiftRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IEnumerable<Gift>> GetAllAsync()
        {
            return await _context.Gifts.Include(x => x.Category).Include(x => x.Donor).Include(x => x.Winner).Include(x => x.purchaseItems).ToListAsync();
        }

        public async Task<Gift?> GetByIdAsync(int id)
        {
            return await _context.Gifts.Include(x => x.Category).Include(x => x.Donor).Include(x => x.Winner).Include(x => x.purchaseItems).FirstOrDefaultAsync(p => p.Id == id);
        }

        // שנה את המתודה הזו ב-GiftRepository.cs
        public async Task<Gift> AddAsync(CreateGiftDTO giftDto, string imageUrl)
        {
            // יצירת הישות עם הנתיב שנשלח מה-Service
            var g = new Gift
            {
                Name = giftDto.Name,
                Description = giftDto.Description,
                DonorId = giftDto.DonorId,
                CategoryId = giftDto.CategoryId,
                ImgUrl = imageUrl // השמת הנתיב הסופי
            };

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
            g.ImgUrl = gift.ImgUrl;
            // לא נוגעים ב-g.Users כאן בכלל כדי לא לדרוס את כל המשתמשים
            await _context.SaveChangesAsync();
            return await GetByIdAsync(g.Id);
        }

        // פעולה ייעודית להוספת משתמש למתנה (אם תרצה)
  /*      public async Task AddUserToGiftAsync(int giftId, int userId)
        {
            var gift = await GetByIdAsync(giftId);
            var user = await _context.Users.FindAsync(userId);
            if (gift != null && user != null && !gift.Users.Any(u => u.Id == userId))
            {
                gift.Users.Add(user);
                await _context.SaveChangesAsync();
            }
        }

        // פעולה ייעודית להסרת משתמש ממתנה (אם תרצה)
      /*  public async Task RemoveUserFromGiftAsync(int giftId, int userId)
        {
            var gift = await GetByIdAsync(giftId);
            var user = await _context.Users.FindAsync(userId);
            if (gift != null && user != null && gift.purchaseItems.Any(u => u.Id == userId))
            {
                gift.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
      */
        // בתוך GiftRepository.cs
        public async Task<bool> RunAllRandomWinnersAsync()
        {
            // שליפת כל המתנות
            var gifts = await _context.Gifts.ToListAsync();
            var rnd = new Random();
            bool changed = false;

            foreach (var g in gifts)
            {
                // תכלס: אם כבר יש זוכה למתנה הזו, אנחנו מדלגים עליה ולא מגרילים שוב
                if (g.WinnerId != null)
                {
                    continue;
                }

                // שליפת המשתמשים שרכשו את המתנה הספציפית הזו מתוך טבלת הרכישות
                var purchaseItems = await _context.PurchaseItems
                                        .Where(pi => pi.GiftId == g.Id)
                                        .ToListAsync();

                if (purchaseItems.Any())
                {
                    var winnerIndex = rnd.Next(purchaseItems.Count);
                    // השמת ה-UserId של הזוכה המאושר
                    g.WinnerId = purchaseItems[winnerIndex].UserId;
                    changed = true;
                }
            }

            if (changed)
            {
                await _context.SaveChangesAsync(); // שמירה רק אם היו שינויים
            }

            return true;
        }
        public async Task<bool> UpdateWinnerAsync(int giftId, int winnerId, bool forceUpdate = false)
        {
            var g = await GetByIdAsync(giftId);
            if (g == null)
            {
                _logger.LogInformation("cannot find gift " + giftId);
                return false;
            }
            // if there's already a winner, do not overwrite (unless forceUpdate is true)
            if (!forceUpdate && g.WinnerId.HasValue)
            {
                _logger.LogInformation("gift " + giftId + " already has a winner");
                return false;
            }
            g.WinnerId = winnerId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Gifts.FindAsync(id);
            if (product == null || product.purchaseItems.Count > 0)
            {
                _logger.LogInformation("users buy thus gift cand delete " + id);
                return false;

            }
            if (product == null)
            {
                _logger.LogInformation("cand find product " + id);
                return false;
            }

            _context.Gifts.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
        public void ClearTracker()
        {
            _context.ChangeTracker.Clear();
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Gifts.AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Gift>> SearchAsync(string? giftName, string? donorName, int? purchaserCount)
        {
            var query = _context.Gifts
                .Include(g => g.Donor)
                .Include(g => g.purchaseItems).
                Include(x => x.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(giftName))
                query = query.Where(g => g.Name.Contains(giftName));

            if (!string.IsNullOrEmpty(donorName))
                query = query.Where(g => g.Donor.Name.Contains(donorName));

            if (purchaserCount.HasValue)
                query = query.Where(g => g.purchaseItems.Count == purchaserCount.Value);

            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Gift>> GetSortedAsync(bool sortByName, bool sortByCategory)
        {
            var query = _context.Gifts.Include(g => g.Donor)
                .Include(g => g.purchaseItems).
                Include(x => x.Category).AsQueryable();


            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Gift>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Gifts
                .Include(g => g.Category)
                .Include(g => g.Donor)
                .Include(g => g.Winner)
                .Where(g => g.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<GiftWinnerReportDTO>> GetGiftWinnersReportAsync()
        {
            var gifts = await _context.Gifts
                .Include(g => g.Winner)
                .ToListAsync();

            return gifts.Select(g => new GiftWinnerReportDTO
            {
                GiftId = g.Id,
                GiftName = g.Name ?? string.Empty,
                WinnerId = g.WinnerId,
                WinnerName = g.Winner != null ? (g.Winner.FirstName + " " + g.Winner.LastName) : string.Empty,
                WinnerEmail = g.Winner != null ? g.Winner.Email : string.Empty
            });
        }
        public async Task<IEnumerable<Gift>> GetSortedGiftsAsync(string sortBy)
        {
            IQueryable<Gift> query = _context.Gifts
           .Include(g => g.purchaseItems).Include(g => g.Category).Include(g => g.Donor);


            switch (sortBy?.ToLower())
            {
                case "purchases":
                    query = query.OrderByDescending(g => g.purchaseItems.Count);
                    break;

                default:
                    query = query.OrderBy(g => g.Name);
                    break;
            }

            return await query.ToListAsync();

        }


    }
}
