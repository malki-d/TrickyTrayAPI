using System.ComponentModel.DataAnnotations;

namespace TrickyTrayAPI.Models
{
    public class Donor
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [EmailAddress,Required]
        public string? Email { get; set; }
        [Phone]
        public string PhoneNumber { get; set; }
        public ICollection<Gift> Gifts { get; set; } = new List<Gift>();

    }
}
