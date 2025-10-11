using Microsoft.AspNetCore.Mvc;

namespace Gym_Membership.Controllers
{
    public class EquipmentController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult New() => View();
        public IActionResult Edit() => View();
    }
}
