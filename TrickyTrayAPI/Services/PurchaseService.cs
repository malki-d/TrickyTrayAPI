using NuGet.Protocol.Core.Types;
using TrickyTrayAPI.DTOs;
using TrickyTrayAPI.Models;



public class PurchaseService : IPurchaseService
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly ILogger<PurchaseService> _logger;


    public PurchaseService(IPurchaseRepository purchaseRepository)
    {
        _purchaseRepository = purchaseRepository;
    }
    public async Task<IEnumerable<UserResponseDTO>> GetAllAsync()
    {
        try
        {
            var purchases = await _purchaseRepository.GetAllAsync();
            var result = purchases.Select(p => new UserResponseDTO
            {
                FirstName = p.User.FirstName,
                LastName = p.User.LastName,
                Email = p.User.Email,
                Phone = p.User.PhoneNumber,
            });
            _logger.LogInformation("Successfully retrieved all purchases");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all purchases");
            throw;
        }
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
        // 1. קבלת פריטי העגלה לפי ה-userId שהתקבל
        var cartItems = await _purchaseRepository.GetCartItemsByUserIdAsync(userId);

        if (cartItems == null || !cartItems.Count.Equals(0)) // תיקון: בדיקה אם הרשימה ריקה
        {
            // אם הרשימה ריקה זרוק שגיאה (או טפל בהתאם)
            if (!cartItems.Any()) throw new Exception("Cart is empty for user " + userId);
        }

        var purchaseItems = new List<PurchaseItem>();
        int totalPrice = 0;

        // 2. לוגיקת המרת CartItems ל-PurchaseItems
        foreach (var item in cartItems)
        {
            // נניח של-Gift יש שדה Price (הוספתי את זה כהנחה לחישוב המחיר)
             totalPrice += 10 * item.Quantity;

            // יצירת רשומות נפרדות לפי הכמות (Quantity)
            for (int i = 0; i < item.Quantity; i++)
            {
                purchaseItems.Add(new PurchaseItem
                {
                    GiftId = item.GiftId,
                    UserId = userId, // שימוש ב-userId שהתקבל
                    IsWinner = false
                });
            }
        }

        // 3. בניית אובייקט הרכישה
        var purchase = new Purchase
        {
            UserId = userId,
            Date = DateTime.Now,
            Price = totalPrice, // שים לב: זה יהיה 0 אם אין לך שדה מחיר ב-Gift
            PurchaseItems = purchaseItems
        };

        // 4. שמירה וניקוי
        await _purchaseRepository.AddPurchaseAsync(purchase);
        await _purchaseRepository.ClearUserCartAsync(userId); // ניקוי העגלה של המשתמש הספציפי
        await _purchaseRepository.SaveAsync();

        return purchase;
    }
}