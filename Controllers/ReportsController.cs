using System;
using System.Linq;
using Gym_Membership.Data;
using Microsoft.AspNetCore.Mvc;
using Gym_Membership.Models; // change to your actual namespace

namespace Gym_Membership.Controllers
{
    public class ReportsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Reports/ExpiredMembers
        public ActionResult ExpiredMembers()
        {
            var expiredMembers = from m in db.Customers
                                 join p in db.Pays on m.CustomerID equals p.CustomerID
                                 join plan in db.Memberships on p.PaysID equals plan.MembershipID
                                 where p.PaymentDate < DateTime.Now
                                 select new ExpiredMemberViewModel
                                 {
                                     MemberID = m.CustomerID,
                                     Name = m.Name,
                                     PlanName = plan.Type,
                                     ExpiryDate = p.PaymentDate
                                 };

            return View(expiredMembers.ToList());
        }
    }
}
