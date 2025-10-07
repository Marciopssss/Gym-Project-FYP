namespace Gym_Membership.Models
{
    public class Role
    {
        public int RoleID { get; set; }
        public string RoleName { get; set; }

        // One-to-One with User
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
