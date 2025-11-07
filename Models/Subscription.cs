using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_Membership.Models
{
    public class Subscription
    {
        [Key]
        public int SubscriptionID { get; set; }

        [Required]
        public int CustomerID { get; set; }

        [ForeignKey("CustomerID")]
        public Customer Customer { get; set; }

        [Required]
        public int MembershipID { get; set; }

        [ForeignKey("MembershipID")]
        public Membership Membership { get; set; }
        public string UserEmail { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
