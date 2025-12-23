using System.ComponentModel.DataAnnotations;

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
}