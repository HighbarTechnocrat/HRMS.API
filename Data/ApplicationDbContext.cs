using HRMS.API.Models;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ErrorLog> ErrorLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ErrorLog entity  
            modelBuilder.Entity<ErrorLog>(static entity =>
            {
                entity.Property(e => e.ControllerName)
                    .HasMaxLength(500)
                    .IsRequired();

                entity.Property(e => e.ActionName)
                    .HasMaxLength(200)
                    .IsRequired();

                entity.Property(e => e.ErrorMessage)
                    .IsRequired();

                entity.Property(e => e.ErrorLevel)
                    .HasMaxLength(50);
                //.HasDefaultValue("Error"); // Correct usage for default value  

                entity.Property(e => e.CreatedAt);
                    //.HasDefaultValueSql("GETUTCDATE()"); // For SQL Server  
            });
 
        }
    }
}
