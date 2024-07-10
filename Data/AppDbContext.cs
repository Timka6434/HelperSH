using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using test_indentity.Models;

namespace test_indentity.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Request> Requests { get; set; }
        public DbSet<Consumable> Consumables { get; set; }
        public DbSet<Equipment> Equipments { get; set; }
        public DbSet<CabinetList> CabinetLists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Request>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Equipment>()
                 .HasOne(e => e.CabinetList)
                 .WithMany(c => c.Equipments)
                 .HasForeignKey(e => e.CabinetId)
                 .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<CabinetList>()
                .HasMany(c => c.Equipments)
                .WithOne(e => e.CabinetList)
                .HasForeignKey(e => e.CabinetId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Request>()
                .HasOne<CabinetList>()
                .WithMany()
                .HasForeignKey(r => r.Room)
                .HasPrincipalKey(c => c.FullCabinetName)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
