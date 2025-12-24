using System.ComponentModel.DataAnnotations;

namespace TrickyTrayAPI.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
        [Required]
        public int Price { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
