namespace TrickyTrayAPI.Models
{
    public class Purchase
    {
        public int Id { get; set; }
        public int CostumerId { get; set; }
        public Costumer Costumer { get; set; }
        public ICollection<PurchaseItem> PurchaseItems { get; set; }
        public int Price { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
