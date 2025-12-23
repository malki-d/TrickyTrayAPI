namespace TrickyTrayAPI.Models
{
    public class Cart
    {
        public int Id { get; set; }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
