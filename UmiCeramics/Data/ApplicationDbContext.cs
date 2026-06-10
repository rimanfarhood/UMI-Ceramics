using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UmiCeramics.Models;

namespace UmiCeramics.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
     public DbSet<Workshop> Workshops { get; set; }

     public DbSet<Booking> Bookings { get; set; }
     protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Workshop>()
            .Property(w => w.Price)
            .HasPrecision(10, 2);
    }
}
