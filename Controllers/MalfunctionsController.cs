using Microsoft.AspNetCore.Mvc;

namespace Gym_Membership.Controllers
{
    public class MalfunctionsController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult New() => View();
        public IActionResult Edit() => View();
        public IActionResult History() => View();
        public IActionResult Details() => View();
    }
}
