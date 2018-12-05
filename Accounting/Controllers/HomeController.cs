using HoneySale.Models;
using HoneySaleDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Accounting.Controllers
{
    [SessionTimeout]
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        honeysaleEntities db = new honeysaleEntities();
        public ActionResult Home()
        {
            ViewBag.DeliveryCount = db.DeliveryDetails.ToList().Count();
            ViewBag.UserCount = db.tblusers.ToList().Count();
            ViewBag.CustomerCount = db.CustomerDetails.ToList().Count();
            return View();
        }

    }
}
