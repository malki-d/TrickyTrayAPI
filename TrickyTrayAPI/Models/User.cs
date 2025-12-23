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
        public string LastName { get; set; }
        public string FirstName { get; set; }
        
        public string? Email { get; set; }

        public string PhoneNumber { get; set; }

        public TypeCostumer TypeCostumer { get; set; } = TypeCostumer.User;


    }
}
