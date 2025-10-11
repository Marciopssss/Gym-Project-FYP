using Microsoft.AspNetCore.Mvc;

namespace Gym_Membership.Controllers
{
    public class ManageController : Controller
    {
        public IActionResult AddPhoneNumber() => View();
        public IActionResult ChangePassword() => View();
        public IActionResult SetPassword() => View();
        public IActionResult LinkLogin() => View();
    }
}
