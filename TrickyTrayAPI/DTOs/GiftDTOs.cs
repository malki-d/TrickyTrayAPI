using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.DTOs
{
    public class GetGiftDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string ImgUrl { get; set; }
        public string Category { get; set; }
        public string DonorName { get; set; }

    }
    public class GetGiftWithWinnerDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string ImgUrl { get; set; }
        public string Category { get; set; }
        public string WinnerName { get; set; }
        [EmailAddress]
        public string WinnerEmail { get; set; }


    }

    public class GiftWinnerReportDTO
    {
        public int GiftId { get; set; }
        public string GiftName { get; set; }
        public int? WinnerId { get; set; }
        public string WinnerName { get; set; }
        public string WinnerEmail { get; set; }
    }

    public class CreateGiftDTO
    {
        [MaxLength(100), Required]
        public string? Name { get; set; }
        [MaxLength(200), Required]
        public string? Description { get; set; }

        // זה השדה שיקבל את הקובץ מה-Form
        [Required]
        public IFormFile ImageFile { get; set; }

        [Required]
        public int DonorId { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
    public class UpdateGiftDTO
    {
        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required, MaxLength(200)]
        public string Description { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();

        // הוסיפי את השדה הזה כדי לקבל קובץ
        public IFormFile? ImageFile { get; set; }

        public string? ImgUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
