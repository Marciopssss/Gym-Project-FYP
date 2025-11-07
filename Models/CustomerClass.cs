using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gym_Membership.Models
{
    public class CustomerClass
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public User User { get; set; }

        [ForeignKey("Class")]
        public int ClassID { get; set; }
        public Classes Class { get; set; }

        public DateTime SubscriptionDate { get; set; }
    }
}
