using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_Membership.Models
{
    public class Staff
    {
        [Key]
        public int StaffID { get; set; }
        public string Name { get; set; }
        public string? Position { get; set; }
        public string? Phone { get; set; }
        public string Email { get; set; }

        // One-to-One with User
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        public User User { get; set; }

        // One-to-Many with Classes
        public ICollection<Classes> Classes { get; set; }

        // One-to-Many with Pays
        public ICollection<Pays> Pays { get; set; }

        // One-to-Many with Feedback
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
