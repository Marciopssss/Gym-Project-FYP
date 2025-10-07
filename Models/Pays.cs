namespace Gym_Membership.Models
{
    public class Pays
    {
        public int PaysID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }

        // One-to-One with Customer
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

        // One-to-One or Many-to-One with Staff
        public int StaffID { get; set; }
        public Staff Staff { get; set; }
    }
}
