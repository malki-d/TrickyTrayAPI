using Microsoft.EntityFrameworkCore;
using TrickyTrayAPI.Models;
using TrickyTrayAPI.Models;

namespace WebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    public DbSet<Category> Categories { get; set; }
    public DbSet<Donor> Donors { get; set; }
    public DbSet<Gift> Gifts { get; set; }

    public DbSet<Costumer> Costumers { get; set; }



}
