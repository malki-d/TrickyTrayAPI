using System.ComponentModel.DataAnnotations;

namespace TrickyTrayAPI.Models
{
    public enum TypeCostumer
    {
        User,
        Admin
    }
    public class User
    {
        public int Id { get; set; }
        [Required,MaxLength(100)]
        public string LastName { get; set; }
        [Required, MaxLength(100)]
        public string FirstName { get; set; }
        [Required, EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public string PhoneNumber { get; set; }
        public TypeCostumer TypeCostumer { get; set; } = TypeCostumer.User;
        public ICollection<CartItem> CartItems { get; set; }
    }
}
