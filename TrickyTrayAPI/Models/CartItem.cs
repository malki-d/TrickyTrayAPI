using System.ComponentModel.DataAnnotations;

namespace TrickyTrayAPI.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; } = 1;
        [Required]
        public int GiftId { get; set; }

        public Gift Gift { get; set; }
        [Required]

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
