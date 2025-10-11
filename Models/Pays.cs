using System.ComponentModel.DataAnnotations;

namespace Gym_Membership.Models
{
    public class Pays
    {
        [Key]
        public int PaysID { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }

        // One-to-One with Customer
        public int CustomerID { get; set; }
        public Customer Customer { get; set; }

       
    }
}
