using NuGet.Protocol.Core.Types;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Repositories;
using TrickyTrayAPI.Services;



public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IGiftRepository _giftRepository;

    private readonly ILogger<PurchaseService> _logger;


    public PurchaseService(IGiftRepository giftRepository,IPurchaseRepository purchaseRepositor, ILogger<PurchaseService> logger)
    {
        _purchaseRepository = purchaseRepositor;
        _logger = logger;
        _giftRepository = giftRepository;
    }
    public async Task<IEnumerable<UserPurchaseDto>> GetAllAsync()
    {
        try
        {
            // שליפת כל הרכישות כולל הנתונים הנלווים
            var purchases = await _purchaseRepository.GetAllAsync();

            var result = purchases.Select(p => new UserPurchaseDto
            {
                PurchaseId = p.Id,
                Date = p.Date, // ודאי שבמודל Purchase קיים שדה Date
                TotalPrice = p.Price,
                TotalTickets = p.PurchaseItems != null ? p.PurchaseItems.Count : 0,
                UserName=p.User.FirstName+" "+p.User.LastName,
                Items = p.PurchaseItems?
                    .GroupBy(pi => pi.GiftId)
                    .Select(g => new PurchasedGiftItemDto
                    {
                        GiftId = g.Key,
                        GiftName = g.First().Gift?.Name ?? "Unknown",
                        ImgUrl = g.First().Gift?.ImgUrl ?? string.Empty,
                        Quantity = g.Count()
                    }).ToList() ?? new List<PurchasedGiftItemDto>()
            }).ToList();

            _logger.LogInformation("Successfully retrieved all purchases for admin");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all purchases");
            throw;
        }
    }



    public async Task<IEnumerable<UserPurchaseDto>> GetUserPurchasesLogic(int userId)
    {
        // 1. שליפת הנתונים מה-DAL
        var purchases = await _purchaseRepository.GetAllAsyncByUserId(userId);

        // 2. המרה ועיבוד לוגי (Mapping & Logic)
        var result = purchases.Select(p => new UserPurchaseDto
        {
            PurchaseId = p.Id,
            Date = p.Date,
            TotalPrice = p.Price,
            // סופר כמה שורות יש סה"כ ב-PurchaseItems עבור הקניה הזו
            TotalTickets = p.PurchaseItems.Count,

            // כאן הקסם קורה: קיבוץ לפי מתנה כדי לקבל "כמות"
            Items = p.PurchaseItems
                .GroupBy(pi => pi.GiftId) // מקבצים לפי מזהה המתנה
                .Select(g => new PurchasedGiftItemDto
                {
                    GiftId = g.Key,
                    // לוקחים את השם והתמונה מהפריט הראשון בקבוצה (הם זהים לכולם)
                    GiftName = g.First().Gift.Name,

                    ImgUrl = g.First().Gift.ImgUrl,
                    // הכמות היא מספר הפריטים בקבוצה הזו
                    Quantity = g.Count()
                }).ToList()
        }).ToList();

        return result;
    }
    public async Task<PurchaseRevenueDTO> GetTotalRevenueAsync()
    {
        try
        {
            var revenue = await _purchaseRepository.GetTotalRevenueAsync();
            _logger.LogInformation("Retrieved total revenue");
            return revenue;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving total revenue");
            throw;
        }
    }
    public async Task<Purchase> ProcessPurchaseAsync(int userId)
    {
        // 1. שליפת פריטי העגלה
        var cartItems = await _purchaseRepository.GetCartItemsByUserIdAsync(userId);

        // בדיקה אם העגלה ריקה בצורה תקינה
        if (cartItems == null || !cartItems.Any())
        {
            _logger.LogWarning("User {UserId} tried to checkout with an empty cart.", userId);
            throw new Exception("העגלה ריקה, לא ניתן לבצע רכישה.");
        }

        var purchaseItems = new List<PurchaseItem>();
        int totalPrice = 0;
        const int TICKET_PRICE = 40; // הגדרה קבועה למחיר כרטיס

        // 2. עיבוד פריטי העגלה והפיכתם לכרטיסים (PurchaseItems)
        foreach (var item in cartItems)
        {
            // חישוב המחיר הכולל
            totalPrice += TICKET_PRICE * item.Quantity;

            // שליפת המתנה כדי לעדכן את הקשרים שלה
            var gift = await _giftRepository.GetByIdAsync(item.GiftId);

            if (gift == null) continue;

            // יצירת כרטיסים נפרדים לפי הכמות
            for (int i = 0; i < item.Quantity; i++)
            {
                var newTicket = new PurchaseItem
                {
                    GiftId = item.GiftId,
                    UserId = userId,
                    IsWinner = false
                    // שימי לב: אם הוספת PurchaseId במודל, EF ימלא אותו לבד כשישייך ל-Purchase
                };

                purchaseItems.Add(newTicket);

                // עדכון רשימת הכרטיסים של המתנה (אופציונלי, תלוי אם את צריכה את זה לקוד בהמשך)
                gift.purchaseItems.Add(newTicket);
            }
        }

        // 3. יצירת אובייקט הרכישה המרכזי
        var purchase = new Purchase
        {
            UserId = userId,
            Date = DateTime.Now,
            Price = totalPrice,
            PurchaseItems = purchaseItems
        };

        try
        {
            // 4. שמירה וניקוי - הכל ב-Transaction אחד דרך ה-Repository
            await _purchaseRepository.AddPurchaseAsync(purchase);

            // ניקוי העגלה
            await _purchaseRepository.ClearUserCartAsync(userId);

            // פקודה אחת שסוגרת את כל השינויים (גם של המתנות וגם של הרכישה)
            await _purchaseRepository.SaveAsync();

            _logger.LogInformation("Purchase {PurchaseId} completed successfully for user {UserId}", purchase.Id, userId);
            return purchase;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process purchase for user {UserId}", userId);
            throw new Exception("ארעה שגיאה בתהליך התשלום, אנא נסה שוב.");
        }
    }
}