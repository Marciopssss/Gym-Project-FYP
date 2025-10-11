using System;
using System.Linq;
using System.Web.Mvc;
using Gym_Membership.Data;
using Microsoft.AspNetCore.Mvc;
using Gym_Membership.Models; // change to your actual namespace

namespace YourProjectName.Controllers
{
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reports/ExpiredMembers
        public ActionResult ExpiredMembers()
        {
            var expiredMembers = from m in db.Customers
                                 join p in db.Pays on m.CustomerID equals p.CustomerID
                                 join plan in db.Memberships on p.MembershipID equals plan.PlanID
                                 where p.ExpiryDate < DateTime.Now
                                 select new ExpiredMemberViewModel
                                 {
                                     MemberID = m.MemberID,
                                     Name = m.Name,
                                     PlanName = plan.PlanName,
                                     ExpiryDate = p.ExpiryDate
                                 };

            return View(expiredMembers.ToList());
        }
    }
}
