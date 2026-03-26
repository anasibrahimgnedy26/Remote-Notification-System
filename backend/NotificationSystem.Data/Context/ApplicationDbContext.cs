using Microsoft.EntityFrameworkCore;
using NotificationSystem.Core.Entities;

namespace NotificationSystem.Data.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<NotificationLog> NotificationLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Notification configuration
            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Body).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.SentBy).HasMaxLength(100);
                entity.Property(e => e.TargetType).IsRequired().HasMaxLength(20).HasDefaultValue("All");
                entity.Property(e => e.TargetValue).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.CreatedAt);
            });

            // Device configuration
            modelBuilder.Entity<Device>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.DeviceToken).IsRequired().HasMaxLength(500);
                entity.Property(e => e.DeviceName).HasMaxLength(200);
                entity.Property(e => e.RegisteredAt).HasDefaultValueSql("GETUTCDATE()");
                entity.HasIndex(e => e.DeviceToken).IsUnique();
            });

            // NotificationLog configuration
            modelBuilder.Entity<NotificationLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasMaxLength(50);
                entity.Property(e => e.ErrorMessage).HasMaxLength(500);
                entity.Property(e => e.FirebaseResponseId).HasMaxLength(200);
                entity.Property(e => e.SentAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(e => e.Notification)
                      .WithMany(n => n.NotificationLogs)
                      .HasForeignKey(e => e.NotificationId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Device)
                      .WithMany(d => d.NotificationLogs)
                      .HasForeignKey(e => e.DeviceId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
