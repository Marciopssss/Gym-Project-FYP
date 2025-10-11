using System.ComponentModel.DataAnnotations;

namespace Gym_Membership.Models
{
    public class Membership
    {
        [Key]
        public int MembershipID { get; set; }

        [Required(ErrorMessage = "Please select a membership type")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Please select a duration")]
        public int? Duration { get; set; }  // ✅ Nullable (matches dropdown)

        [Required(ErrorMessage = "Please select a price")]
        public decimal? Price { get; set; }  // ✅ Nullable (matches dropdown)

        // One-to-Many with Customers
        public ICollection<Customer> Customers { get; set; }
    }
}

