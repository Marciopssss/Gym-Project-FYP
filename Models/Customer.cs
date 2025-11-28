using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_Membership.Models
{
    public class Customer
    {
        [Key]
        public int CustomerID { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // One-to-One with User (account owner)
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        // One-to-One with Membership
        public int? MembershipID { get; set; }
        public Membership Membership { get; set; }

        // One-to-One with Pays
        public Pays Pays { get; set; }

        // 🔹 Personal trainer relationship (many customers → one trainer)
        //  NO [ForeignKey] attribute here now
        public int? PersonalTrainerId { get; set; }   // FK column in Customers table
        public User? PersonalTrainer { get; set; }    // Navigation to trainer User

        // 🔹 Personal training session time
        public DateTime? PersonalTrainingTime { get; set; }

        // One-to-Many with Classes  (this part is a bit weird but leave it for now)
        public int? ClassID { get; set; }
        [ForeignKey("ClassID")]
        public ICollection<Classes> Classes { get; set; }

        // One-to-Many with Feedback
        public ICollection<Feedback> Feedbacks { get; set; }

        public bool IsActive { get; set; } = true;
        public ICollection<Subscription>? Subscriptions { get; set; }
    }
}
