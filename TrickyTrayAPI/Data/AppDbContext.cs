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
    public DbSet<TicketPrice> TicketPrices { get; set; }

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
        modelBuilder.Entity<TicketPrice>().HasData(
        new TicketPrice { Id = 1, Price = 1 }
        );
        // הגדרת הקשר בין Gift ל-PurchaseItem בצורה מפורשת
        modelBuilder.Entity<PurchaseItem>()
            .HasOne(pi => pi.Gift)
            .WithMany(g => g.purchaseItems)
            .HasForeignKey(pi => pi.GiftId)
            .OnDelete(DeleteBehavior.Restrict); // מומלץ כדי למנוע שגיאות מחיקה בשרשרת

        // הגדרת הקשר בין Gift ל-Winner
        modelBuilder.Entity<Gift>()
            .HasOne(g => g.Winner)
            .WithMany() // למשתמש אין רשימת זכיות במודל הנוכחי, וזה בסדר
            .HasForeignKey(g => g.WinnerId);

        modelBuilder.Entity<PurchaseItem>()
        .HasOne(pi => pi.User)
        .WithMany()
        .HasForeignKey(pi => pi.UserId)
        .OnDelete(DeleteBehavior.Restrict); // <--- זה הפתרון!

        // אם הוספת שדה PurchaseId ל-PurchaseItem כפי שהצעתי קודם:
        modelBuilder.Entity<PurchaseItem>()
            .HasOne(pi => pi.Gift)
            .WithMany(g => g.purchaseItems)
            .HasForeignKey(pi => pi.GiftId)
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(modelBuilder);
    }


}
