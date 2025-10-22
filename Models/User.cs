using System.ComponentModel.DataAnnotations;

namespace Gym_Membership.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        //[Required]
        public string? Password { get; set; }

        public string? Email { get; set; }
        public string? FullName { get; set; }
        [Phone]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        [Range(10, 100)]
        public int? Age { get; set; }

        public string? Gender { get; set; }


        // ✅ Keep this: simple string role column
        public string Role { get; set; } = "User";

        // ✅ Keep optional relations (not RoleEntity)
        public virtual Admin? Admin { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Staff? Staff { get; set; }
    }
}