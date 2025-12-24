using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.Models;
namespace WebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Category> Categories { get; set; }
    public DbSet<Donor> Donors { get; set; }
    public DbSet<Gift> Gifts { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Purchase> Purchases { get; set; }

    public DbSet<PurchaseItem> PurchaseItems { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CartItem>()
        .HasOne(c => c.Gift)
        .WithMany()
        .HasForeignKey(c => c.GiftId)
        .OnDelete(DeleteBehavior.NoAction);
        modelBuilder.Entity<PurchaseItem>()
       .HasOne(p => p.Gift)
       .WithMany()
       .HasForeignKey(p => p.GiftId)
       .OnDelete(DeleteBehavior.NoAction);
    }

}
