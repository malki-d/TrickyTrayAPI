namespace TrickyTrayAPI.Models
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        public int GiftId { get; set; }
        public Gift Gift{ get; set; }
        public bool IsWinner { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
