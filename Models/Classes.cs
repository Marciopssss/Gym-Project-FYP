using System.ComponentModel.DataAnnotations;

namespace Gym_Membership.Models
{
    public class Classes
    {
        [Key]
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public DateTime ScheduleTime { get; set; }
        public int Duration { get; set; }
        public string TrainerName { get; set; }

        // Relationships
        public ICollection<Customer> Customers { get; set; }
        public int? StaffID { get; set; }
        public Staff Staff { get; set; }
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
