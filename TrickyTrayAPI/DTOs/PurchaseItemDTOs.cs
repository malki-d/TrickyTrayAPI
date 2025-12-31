using TrickyTrayAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace TrickyTrayAPI.DTOs
{
    public class CreatePurchaseItemDTO
    {
        [Required]
        public int GiftId { get; set; }
        [Required]
        public int UserId { get; set; }
    }

    public class UpdatePurchaseItemDTO
    {
        public int GiftId { get; set; }
        public int UserId { get; set; }
        public bool IsWinner { get; set; }
    }

    public class GetPurchaseItemDTO
    {
        public int Id { get; set; }
        public int GiftId { get; set; }
        public string GiftName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool IsWinner { get; set; }
    }
}
