
using CheckinPPP.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace CheckinPPP.Data
{
    public class ApplicationDbContext : DbContext
    {
        DbSet<Member> Members { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
