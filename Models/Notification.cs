using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_Membership.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime DateSent { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;
    }
}
