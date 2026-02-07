using TrickyTrayAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace TrickyTrayAPI.DTOs
{
    public class GetPurchaseDTO
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int Price { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }

    public class CreatePurchaseDTO
    {
        [Required]
        public int UserId { get; set; }
       
        [Required]
        public int Price { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }

    public class PurchaseRevenueDTO
    {
        public int TotalRevenue { get; set; }
        public DateTime AsOf { get; set; }
    }
    public class UserPurchaseDto
    {
        public int PurchaseId { get; set; }
        public DateTime Date { get; set; }
        public int TotalPrice { get; set; } // המחיר הכולל של הקניה
        public int TotalTickets { get; set; } // סה"כ כרטיסים בקניה זו
        public List<PurchasedGiftItemDto> Items { get; set; } // פירוט המוצרים
    }

    // DTO שמייצג פריט בתוך הקניה (מתנה וכמות הכרטיסים שלה)
    public class PurchasedGiftItemDto
    {
        public int GiftId { get; set; }
        public string GiftName { get; set; }
        public string ImgUrl { get; set; }
        public int Quantity { get; set; } // הכמות שחישבנו
    }
}  

