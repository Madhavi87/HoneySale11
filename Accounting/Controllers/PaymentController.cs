
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using HoneySaleDAL;
using System.Threading.Tasks;
using System.IO;
using HoneySale.Models;

namespace HoneySale.Controllers
{
    [SessionTimeout]
    public class PaymentController : Controller
    {
        honeysaleEntities db = new honeysaleEntities();
        StaffPaymentRepository repo = new StaffPaymentRepository(new honeysaleEntities());
        StaffPayment dd = new StaffPayment();


        public ActionResult Listing()
        {
            var StaffPaymentlist = repo.GetAllStaffPaymentDetail();

            return View(StaffPaymentlist);
        }

      
        public ActionResult Create()
        {
            try
            {
                
                dd.staff = repo.GetAllStaff();

                return View(dd);
            }
            catch (Exception)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";

            }

            return RedirectToAction("Listing");
        }

        [HttpPost]
        public ActionResult Create(StaffPayment obj)
        {
            var staffPaymentDetails = repo.GetAllStaffPaymentDetail();
            obj.Received_By = Convert.ToString(Session["UserName"]);
            obj.Received_On = System.DateTime.Now;
            obj.staff = repo.GetAllStaff();
            try
            {
               if (ModelState.IsValid)
                {
                    repo.Insert(obj);
                    ViewBag.SuccessMsg = "Record Added Successfully";
                    staffPaymentDetails = repo.GetAllStaffPaymentDetail();
                    return View("Listing", staffPaymentDetails);
                }
                else
                {
                    return View(obj);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";
                staffPaymentDetails = repo.GetAllStaffPaymentDetail();
                return View("Listing", staffPaymentDetails);

            }
        }

        public ActionResult Edit(int id)
        {
            try
            {

                var staffPaymentDetails = repo.GetStaffPaymentDetailByID(id);

                staffPaymentDetails.staff = repo.GetAllStaff();
                return View(staffPaymentDetails);
            }
            catch (Exception)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";

            }
            return RedirectToAction("Listing");

        }


        [HttpPost]
        public ActionResult Edit(StaffPayment obj)
        {
            obj.Received_By = Convert.ToString(Session["UserName"]);
            obj.Received_On = System.DateTime.Now;

            try
            {
                if (ModelState.IsValid)
                {
                    
                    obj.staff = repo.GetAllStaff();

                    repo.Update(obj);
                    ViewBag.SuccessMsg = "Record Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";
            }
            var staffPaymentDetails = repo.GetAllStaffPaymentDetail();

            return View("Listing", staffPaymentDetails);
        }

        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                //   repo.Delete(id);
                repo.Delete(id);
                ViewBag.SuccessMsg = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";
            }

            var staffPaymentDetail = repo.GetAllStaffPaymentDetail();
            return View("Listing", staffPaymentDetail);
        }
   
    }
}

