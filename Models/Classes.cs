namespace Gym_Membership.Models
{
    public class Classes
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public DateTime ScheduleTime { get; set; }
        public int Duration { get; set; } 
        public string TrainerName { get; set; }

        // Many-to-Many with Customers
        public ICollection<Customer> Customers { get; set; }

        // One-to-Many with Staff
        public int? StaffID { get; set; }
        public Staff Staff { get; set; }

        // One-to-Many with Feedback
        public ICollection<Feedback> Feedbacks { get; set; }
    }
}
