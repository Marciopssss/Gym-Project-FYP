using System;
using System.ComponentModel.DataAnnotations;

namespace Gym_Membership.Models
{
    public class TrainerApplication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }
        

        [Required]
        public string Experience { get; set; }

        public string? CertificatePath { get; set; }

        public string? AdditionalInfo { get; set; }

        public string Status { get; set; } = "Pending"; // Pending / Approved / Rejected

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
