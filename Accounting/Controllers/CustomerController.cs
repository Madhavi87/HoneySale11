
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoneySale.Models;
using HoneySaleDAL;


namespace HoneySale.Controllers
{
    [SessionTimeout]
    public class CustomerController : Controller
    {
        honeysaleEntities db = new honeysaleEntities();
        CustomerRepository repo = new CustomerRepository(new honeysaleEntities());
        CustomerDetail dd = new CustomerDetail();

        public ActionResult Listing()
        {
            var CusotmerMasterlist = repo.GetAllCustomers();

            return View(CusotmerMasterlist);
        }

        public ActionResult CustomerCylinderReport()
        {
            var CusotmerMasterlist = repo.GetAllCustomers().Select(a => new { a.cust_id, a.companyName }).ToList();
            ViewBag.CustomerNames = new SelectList(CusotmerMasterlist, "cust_id", "companyName");

            return View();
        }

        public ActionResult CustomerPaymentReport()
        {
            var CusotmerMasterlist = repo.GetAllCustomers().Select(a => new { a.cust_id, a.companyName }).ToList();
            ViewBag.CustomerNames = new SelectList(CusotmerMasterlist, "cust_id", "companyName");

            return View();
        }

        [HttpPost]
        public ActionResult CustomerPaymentReport(string FromDate, string ToDate, string CustomerNames)
        {
            //string id;
            var customers = repo.GetAllCustomers().Select(a => new { a.cust_id, a.companyName }).ToList();
            ViewBag.CustomerNames = new SelectList(customers,"cust_id", "companyName");



            if (FromDate != "" && ToDate != "" && CustomerNames != "")
            {
                int custid = Convert.ToInt32(CustomerNames);
                var from = Convert.ToDateTime(FromDate);
                var to = Convert.ToDateTime(ToDate);
                var CusotmerMasterlist = db.CustomerPaymentDetails.Where(a => a.date >= from && a.date <= to && a.CustomerDetail.cust_id == custid).ToList();
                return View(CusotmerMasterlist);
            }
            else if (FromDate != "" && ToDate != "" || CustomerNames == "")
            {
                var from = Convert.ToDateTime(FromDate);
                var to = Convert.ToDateTime(ToDate);
                var CusotmerMasterlist = db.CustomerPaymentDetails.Where(a => a.date >= from && a.date <= to);
                return View(CusotmerMasterlist);
            }
            else
            {

                if (CustomerNames != "" && (FromDate == "" || ToDate == ""))
                {
                    int custid = Convert.ToInt32(CustomerNames);
                    var CusotmerMasterlist = db.CustomerPaymentDetails.Where(a => a.cust_Id == custid);
                    return View(CusotmerMasterlist);

                }
            }
            return View();

        }


        [HttpPost]
        public ActionResult CustomerCylinderReport(string CustomerNames)
        {
            var CusotmerMasterlist = repo.GetAllCustomers().Select(a => new { a.cust_id, a.companyName }).ToList();
            ViewBag.CustomerNames = new SelectList(CusotmerMasterlist, "cust_id", "companyName");

            if (CustomerNames != null)
            {
                var CusotmerCylinderMasterlist = repo.GetCustomerCylinderDetail(Convert.ToInt32(CustomerNames));

                return View(CusotmerCylinderMasterlist);
            }
            return View();

        }


        public ActionResult Create()
        {
            dd.cylinders = repo.GetCylinderType();
            dd.CustomerCylinderDetail = new List<CustomerCylinderDetail> { new CustomerCylinderDetail { } };
            return View(dd);
        }

        [HttpPost]
        public JsonResult Create(CustomerDetail obj)
        {
            bool status = false;
            string vMeesage = "";
            //dd.cylinders = repo.GetCylinderType();
            //if (obj.CustomerCylinderDetail != null)
            //{
            //    dd.CustomerCylinderDetail = obj.CustomerCylinderDetail;
            //}
            //else
            //{
            //    dd.CustomerCylinderDetail = new List<CustomerCylinderDetail> { new CustomerCylinderDetail { } };
            //}
            try
            {
                if (ModelState.IsValid)
                {
                    if (repo.GetAllCustomers().Where(ite => ite.companyName.ToLower() == obj.companyName.ToLower()).FirstOrDefault() != null)
                    {
                        status = false;
                        vMeesage = "Customer already exists with same name";
                    }
                    else
                    {
                        CustomerCylinderDetail custCylinder = new CustomerCylinderDetail();
                        if (obj.CustomerCylinderDetail != null)
                        {
                            repo.Insert(obj);
                            status = true;
                        }
                        vMeesage = "Record Added Successfully";


                    }
                    //  var CustomerMasterlist = repo.GetAllCustomers();
                    //return Json(new { status = status });
                }
                else
                {
                    status = false;
                    vMeesage = "Please fill mandatory fields";
                }
            }
            catch (Exception ex)
            {
                vMeesage = "Sorry Some Problem Occured";
                status = false;
                // dd.cylinders = repo.GetCylinderType();
                // dd.CustomerCylinderDetail = obj.CustomerCylinderDetail;

                // return View(dd);

            }

            return new JsonResult { Data = new { status = status, Message = vMeesage } };
        }

        public ActionResult Edit(int id)
        {

            try
            {
                var CustomerMasters = repo.GetCustomerByID(id);
                CustomerMasters.cylinders = repo.GetCylinderType();
                CustomerMasters.CustomerCylinderDetail = repo.GetCustomerCylinderDetail(id);
                CustomerMasters.cust_id = id;
                CustomerMasters.CustomerCylinderDetailForEdit = new List<CustomerCylinderDetail> { new CustomerCylinderDetail { } };
                return View(CustomerMasters);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";

            }

            return RedirectToAction("Listing");
        }


        [HttpPost]
        public ActionResult Edit(CustomerDetail obj)
        {
            bool status = false;
            string vMeesage = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (repo.GetAllCustomers().Where(ite => ite.companyName.ToLower() == obj.companyName.ToLower()
                    && obj.cust_id != ite.cust_id
                    ).FirstOrDefault() != null)
                    {
                        status = false;
                        vMeesage = "Customer already exists with same name";
                    }
                    else
                    {

                        // obj.cylinders = repo.GetCylinderType();
                        repo.Update(obj);
                        vMeesage = "Record Updated Successfully";
                        status = true;
                    }

                }
                else
                {
                    status = false;
                    vMeesage = "Please fill mandatory fields";
                }
            }
            catch (Exception ex)
            {
                vMeesage = "Sorry Some Problem Occured";
                status = false;

            }

            return new JsonResult { Data = new { status = status, Message = vMeesage } };
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

            var CustomerMasterlist = repo.GetAllCustomers();
            return View("Listing", CustomerMasterlist);
        }

        [HttpPost]
        public JsonResult DeleteDetail(int? val)
        {
            try
            {
                //   repo.Delete(id);
                int id = Convert.ToInt32(val);
                repo.DeleteDetail(id);
                return Json(new { Success = "true" });
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";
                return Json(new { Success = "false" });
            }

        }

        /*   public JsonResult GetCustomerName( string term = "")
           {
               var objCustomerlist = repo.GetAllCustomers()
                               .Where(c => c.companyName.ToUpper()
                               .Contains(term.ToUpper()))
                               .Select(c => new { companyName = c.companyName, cust_id = c.cust_id })
                               .Distinct().ToList();
               return Json(objCustomerlist, JsonRequestBehavior.AllowGet);
           }*/

        [HttpPost]
        public JsonResult GetCustomerName(string prefix)
        {
            var objCustomerlist = (from customer in repo.GetAllCustomers()
                                   where customer.companyName.StartsWith(prefix)
                                   select new
                                   {
                                       label = customer.companyName,
                                       val = customer.cust_id
                                   }).ToList();

            return Json(objCustomerlist);
        }
    }
}
