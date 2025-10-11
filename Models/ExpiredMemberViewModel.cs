using System;

namespace Gym_Membership.Models
{
    public class ExpiredMemberViewModel
    {
        public int MemberID { get; set; }
        public string Name { get; set; }
        public string PlanName { get; set; }
        public DateTime ExpiryDate { get; set; }
    }
}
