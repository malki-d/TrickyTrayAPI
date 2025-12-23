using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.DTOs
{
    public class GetGiftDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
        public string DonorName { get; set; }

    }
    public class CreateGiftDTO
    {
        [MaxLength(100), Required]
        public string? Name { get; set; }
        [MaxLength(200), Required]
        public string? Description { get; set; }
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

        [Required]
        public int CategoryId { get; set; }
    }
}
