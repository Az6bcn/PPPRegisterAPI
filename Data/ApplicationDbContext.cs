
using System.Security.Cryptography.X509Certificates;
using CheckinPPP.Data.Entities;
using CheckinPPP.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CheckinPPP.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        DbSet<Member> Members { get; set; }
        DbSet<Booking> Bookings { get; set; }
        DbSet<CancelledBooking> CancelledBookings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Booking>()
                .HasOne(x => x.User)
                .WithMany(y => y.Bookings)
                .HasForeignKey(x => x.UserId);

            builder.Entity<CancelledBooking>()
               .HasOne(x => x.User)
               .WithMany(y => y.CancelledBookings)
               .HasForeignKey(x => x.UserId);
        }

    }
}
