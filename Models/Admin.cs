using System.ComponentModel.DataAnnotations;

namespace Gym_Membership.Models
{
    public class Admin
    {
        [Key]
        public int IdAdmin { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        // One-to-One with User
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
