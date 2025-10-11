using Microsoft.AspNetCore.Mvc;

namespace Gym_Membership.Controllers
{
    public class SupplementsController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult New() => View();
        public IActionResult Edit() => View();
    }
}
