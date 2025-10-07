using System.ComponentModel.DataAnnotations;

namespace Gym_Membership.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public ICollection<Role> Roles { get; set; }

        // One-to-One relationships
        public Admin Admin { get; set; }
        public Customer Customer { get; set; }
        public Staff Staff { get; set; }
    }
}
