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
        public DbSet<Item> Items { get; set; }
        public DbSet<CustomIdFormat> CustomIdFormats { get; set; }
        public DbSet<CustomIdElement> CustomIdElements { get; set; }
        public DbSet<CustomField> CustomFields { get; set; }
        public DbSet<CustomFieldValue> CustomFieldValues { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Inventory>()
                .HasOne(i => i.Creator)
                .WithMany(u => u.OwnedInventories)
                .HasForeignKey(i => i.CreatorId)
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

            builder.Entity<Inventory>()
                .HasOne(i => i.CustomIdFormat)
                .WithOne(c => c.Inventory)
                .HasForeignKey<CustomIdFormat>(c => c.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Inventory>()
                .HasMany(i => i.CustomFields)
                .WithOne(cf => cf.Inventory)
                .HasForeignKey(cf => cf.InventoryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Item>()
                .HasMany(i => i.CustomFieldValues)
                .WithOne(cfv => cfv.Item)
                .HasForeignKey(cfv => cfv.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CustomIdFormat>()
                .HasIndex(c => c.InventoryId)
                .IsUnique();

            builder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
