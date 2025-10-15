using System.ComponentModel.DataAnnotations;

namespace Gym_Membership.Models
{
    public class Classes
    {
        [Key]
        public int ClassID { get; set; }

        [Required(ErrorMessage = "Class name is required")]
        public string ClassName { get; set; }

        [Required(ErrorMessage = "Schedule time is required")]
        public DateTime ScheduleTime { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Trainer name is required")]
        public string TrainerName { get; set; }

        // ✅ Optional relationships — not required when creating a class
        public int? StaffID { get; set; }
        public Staff? Staff { get; set; }

        public ICollection<Customer>? Customers { get; set; } = new List<Customer>();
        public ICollection<Feedback>? Feedbacks { get; set; } = new List<Feedback>();
    }
}
