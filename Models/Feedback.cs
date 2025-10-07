namespace Gym_Membership.Models
{
    public class Feedback
    {
        public int FeedbackID { get; set; }
        public int CustomerID { get; set; } 
        public int? StaffID { get; set; }   
        public int Rating { get; set; }     
        public string Comment { get; set; }
        public DateTime DateSubmitted { get; set; }

        public Customer Customer { get; set; }
        public Staff Staff { get; set; }
        public Classes Classes { get; set; }
    }
}
