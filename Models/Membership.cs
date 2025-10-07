namespace Gym_Membership.Models
{
    public class Membership
    {
        public int MembershipID { get; set; }
        public string Type { get; set; } 
        public int Duration { get; set; } 
        public decimal Price { get; set; }

        // One-to-Many with Customers
        public ICollection<Customer> Customers { get; set; }
    }
}

