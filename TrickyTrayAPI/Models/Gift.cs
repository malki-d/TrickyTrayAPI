using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace TrickyTrayAPI.Models
{

    public class Gift
    {
        public int Id { get; set; }

        [MaxLength(100), Required]
        public string? Name { get; set; }
        [MaxLength(200), Required]
        public string? Description { get; set; }

        [Required]
        public int DonorId { get; set; }
        public Donor? Donor { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        [AllowNull]
        public int? WinnerId { get; set; }
        public User? Winner { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
