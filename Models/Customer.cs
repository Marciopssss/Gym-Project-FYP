using System.ComponentModel.DataAnnotations;

namespace Gym_Membership.Models
{
    public class Customer
    {
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        // One-to-One with User
        public int UserID { get; set; }
        public User User { get; set; }

        // One-to-One with Membership
        public int MembershipID { get; set; }
        public Membership Membership { get; set; }

        // One-to-One with Pays
        public Pays Pays { get; set; }

        // One-to-Many with Classes
        public ICollection<Classes> Classes { get; set; }

        // One-to-Many with Feedback
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
