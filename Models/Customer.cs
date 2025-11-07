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

        // One-to-One with User
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        // One-to-One with Membership
        public int? MembershipID { get; set; }
        public Membership Membership { get; set; }

        // One-to-One with Pays
        public Pays Pays { get; set; }

        // One-to-Many with Classes
        public int? ClassID { get; set; }
        [ForeignKey("ClassID")]
        public ICollection<Classes> Classes { get; set; }

        // One-to-Many with Feedback
        public ICollection<Feedback> Feedbacks { get; set; }

        // ✅ NEW: Manual activation/deactivation flag
        public bool IsActive { get; set; } = true;
        public ICollection<Subscription>? Subscriptions { get; set; }

    }
}
