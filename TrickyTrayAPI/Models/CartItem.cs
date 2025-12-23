namespace TrickyTrayAPI.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public int GiftId { get; set; }

        public Gift Gift { get; set; }

        public int CartId { get; set; }

        public Cart Cart { get; set; }





    }
}
