using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using CourseWork.Models;
using Microsoft.EntityFrameworkCore;


namespace CourseWork.Data
{
    public class CourseWorkDbContext : IdentityDbContext<User>
    {

        public CourseWorkDbContext(DbContextOptions<CourseWorkDbContext> options) 
            : base(options) { }


        public DbSet<Inventory> Inventories { get; set; } 
        public DbSet<Permission> Permissions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Inventory>()
                .HasOne(i => i.User)
                .WithMany(u => u.OwnedInventories)
                .HasForeignKey(i => i.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Permission>()
                .HasOne(p => p.User)
                .WithMany(u => u.Permissions)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Permission>()
                .HasOne(p => p.Inventory)
                .WithMany(i => i.Permissions)
                .HasForeignKey(p => p.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
