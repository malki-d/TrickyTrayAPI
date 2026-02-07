using System.ComponentModel.DataAnnotations;
using TrickyTrayAPI.Models;

namespace TrickyTrayAPI.DTOs
{
    public class CreateDonorDTO
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        [EmailAddress, Required]
        public string? Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
    }
    public class GetDonorDTO
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
    public class GetDonorWithGiftsDTO
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [EmailAddress,Required]
        public string? Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public ICollection<GetGiftDTO> Gifts { get; set; } = new List<GetGiftDTO>();
        public string GiftsString { get; set; } = "";

    }

}