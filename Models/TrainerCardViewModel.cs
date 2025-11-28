namespace Gym_Membership.Models.ViewModels
{
    public class TrainerCardViewModel
    {
        public int TrainerUserId { get; set; }
        public string Name { get; set; }
        public decimal? Price { get; set; }
        public string? Schedule { get; set; }
    }
}
