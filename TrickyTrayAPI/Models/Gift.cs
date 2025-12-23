namespace TrickyTrayAPI.Models
{
    public enum TypeGift
    {
        Regular,
        Premium
    }
public class Gift
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        public TypeGift TypeGift { get; set; } = TypeGift.Premium;


        public int DonorId { get; set; }
        public Donor Donor { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public int WinnerId { get; set; }

        public User Winner { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();


    }
}
