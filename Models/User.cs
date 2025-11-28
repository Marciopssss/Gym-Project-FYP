using System.ComponentModel.DataAnnotations;

namespace Gym_Membership.Models
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string Username { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }
        public string? FullName { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public string? Address { get; set; }

        [Range(10, 100)]
        public int? Age { get; set; }

        public string? Gender { get; set; }

        // ✅ User Role (Admin / User)
        public string Role { get; set; } = "User";

        // ✅ Optional relationships
        public virtual Admin? Admin { get; set; }
        public virtual Customer? Customer { get; set; }
        public virtual Staff? Staff { get; set; }

        // 🏋️ Added for Trainer feature
        public bool IsTrainer { get; set; } = false;
        public decimal? TrainerPrice { get; set; }            // price per month / package
        public string? TrainerSchedule { get; set; }

        // 💬 Optional notification for system messages
        public string? Notification { get; set; }
        public string? TrainerNotification { get; set; }
    }
}
