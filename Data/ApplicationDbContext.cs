using Gym_Membership.Models;
using Microsoft.EntityFrameworkCore;

namespace Gym_Membership.Data
{
    public class ApplicationDbContext : DbContext
    {
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
    }
}
