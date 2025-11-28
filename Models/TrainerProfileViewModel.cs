// Models/ViewModels/TrainerProfileViewModel.cs
using System.Collections.Generic;

namespace Gym_Membership.Models
{
    public class TrainerProfileViewModel
    {
        public User CurrentTrainer { get; set; }

        // All trainers in the gym
        public List<User> AllTrainers { get; set; } = new();

        // Clients assigned to THIS trainer
        public List<Customer> MyClients { get; set; } = new();
    }
}
