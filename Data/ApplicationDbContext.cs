using Gym_Membership.Models;
using Microsoft.EntityFrameworkCore;

namespace Gym_Membership.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Pays> Pays { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Classes> Classes { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<CustomerClass> CustomerClasses { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TrainerApplication> TrainerApplications { get; set; }




        // 🔹 Add this method BELOW all DbSets
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ✅ Correct one-to-many between Customer → Subscriptions
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Customer)
                .WithMany(c => c.Subscriptions)
                .HasForeignKey(s => s.CustomerID)
                .OnDelete(DeleteBehavior.Restrict);

            // ✅ Keep membership relation as before
            modelBuilder.Entity<Subscription>()
                .HasOne(s => s.Membership)
                .WithMany()
                .HasForeignKey(s => s.MembershipID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Customer>()
              .HasOne(c => c.User)
              .WithOne()                        // no nav back on User
              .HasForeignKey<Customer>(c => c.UserId)
              .OnDelete(DeleteBehavior.Restrict);

            // ✅ Customer → PersonalTrainer (many customers per trainer)
            modelBuilder.Entity<Customer>()
                .HasOne(c => c.PersonalTrainer)
                .WithMany()                      // trainer doesn't have a collection nav
                .HasForeignKey(c => c.PersonalTrainerId)
                .OnDelete(DeleteBehavior.SetNull);
        }
       

    }
}
