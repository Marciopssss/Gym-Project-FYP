using Microsoft.AspNetCore.Mvc;

namespace Gym_Membership.Controllers
{
    public class FlavorsController : Controller
    {
        public IActionResult Index() => View();
    }
}
