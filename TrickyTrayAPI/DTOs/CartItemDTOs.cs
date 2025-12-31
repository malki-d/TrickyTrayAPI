using System.ComponentModel.DataAnnotations;

namespace TrickyTrayAPI.DTOs
{
    // CartItemDto.cs
    public class GetCartItemDTO
    {
        public string UserName { get; set; }
        public string GiftName { get; set; }
        public int Quantity { get; set; }
    }
    public class UpdateCartItemDTO
    {
        [Required]
        public int id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required]
        public int GiftId { get; set; }
        public int Quantity { get; set; } = 1;
    }

    // CartItemCreateDto.cs
    public class CreateCartItemDTO
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int GiftId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
