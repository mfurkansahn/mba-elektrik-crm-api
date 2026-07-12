using MbaCrm.Api.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MbaCrm.Api.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        public DbSet<ServiceRequestNote> ServiceRequestNotes { get; set; }

        public DbSet<ServiceRequestDocument> ServiceRequestDocuments { get; set; }

        public DbSet<ServiceRequestReminder> ServiceRequestReminders { get; set; }

        protected override void OnModelCreating(
    ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .HasOne(user => user.Customer)
                .WithOne()
                .HasForeignKey<ApplicationUser>(
                    user => user.CustomerId
                )
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}