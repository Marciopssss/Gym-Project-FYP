using System;
using System.Collections.Generic;
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
        public int? Duration { get; set; }  // in months

        [Required(ErrorMessage = "Please select a price")]
        public decimal? Price { get; set; }

        // ✅ NEW: Start and Expiry Dates
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime ExpiryDate
        {
            get
            {
                // Automatically calculated based on duration (months)
                return StartDate.AddMonths(Duration ?? 0);
            }
        }

        // Relationships
        public ICollection<Customer> Customers { get; set; }
    }
}
