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
}  

