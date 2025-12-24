namespace TrickyTrayAPI.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int GiftId { get; set; }

        public Gift Gift { get; set; }

        public int UserId { get; set; }

        public User User { get; set; }
    }
}
