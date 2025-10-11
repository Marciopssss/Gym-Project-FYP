using Microsoft.AspNetCore.Mvc;

namespace Gym_Membership.Controllers
{
    public class EmployeesController : Controller
    {
        public IActionResult Index() => View();
        public IActionResult Details() => View();
        public IActionResult Edit() => View();
    }
}
