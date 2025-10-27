using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_Membership.Models
{
    public class Membership
    {
        [Key]
        public int MembershipID { get; set; }

        [Required(ErrorMessage = "Please select a membership type")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Please select a duration")]
        public int Duration { get; set; }  // ✅ Non-nullable now

        public string description   { get; set; }
        public string? ImagePath { get; set; }

        [Required(ErrorMessage = "Please select a price")]
        [Column(TypeName = "decimal(10,2)")]  // ✅ Prevents truncation warning
        public decimal Price { get; set; }

        // ✅ Start and calculated expiry date
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [NotMapped]  // ✅ Do not store ExpiryDate in the DB
        public DateTime ExpiryDate => StartDate.AddMonths(Duration);

        // ✅ Initialize navigation properties to avoid null refs
        public ICollection<Customer>? Customers { get; set; } = new List<Customer>();
    }
}
