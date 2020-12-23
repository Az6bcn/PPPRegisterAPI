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

        private DbSet<Member> Members { get; set; }
        private DbSet<Booking> Bookings { get; set; }
        private DbSet<CancelledBooking> CancelledBookings { get; set; }

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