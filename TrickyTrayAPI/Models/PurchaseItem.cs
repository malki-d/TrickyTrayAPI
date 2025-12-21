namespace TrickyTrayAPI.Models
{
    public class PurchaseItem
    {
        public int Id { get; set; }
        public int GiftId { get; set; }
        public Gift Gift{ get; set; }
        public bool IsWinner { get; set; }
        public int CostumerId { get; set; }
        public Costumer Costumer { get; set; }
    }
}
