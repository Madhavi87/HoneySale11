
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
    public class CylinderController : Controller
    {
        CylinderRepository repo = new CylinderRepository(new honeysaleEntities());

        LiveCylinderDetailRepository repo_Live = new LiveCylinderDetailRepository(new honeysaleEntities());

        public ActionResult Listing()
        {
            var CusotmerMasterlist = repo.GetAllCylinders();

            return View(CusotmerMasterlist);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(CylinderMaster obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var cylindertypelist = repo.GetAllCylinders();
                    int count = cylindertypelist.Where(ite => ite.cylinderType.ToLower() == obj.cylinderType.ToLower()).ToList().Count();

                    if (count <= 0)
                    {
                        obj.CreatedBy = Convert.ToString(Session["UserName"]);
                        obj.CreatedOn = System.DateTime.Now;
                        repo.Insert(obj);
                        ViewBag.SuccessMsg = "Record Added Successfully";
                      //  var CusotmerMasterlist = repo.GetAllCylinders();

                        return RedirectToAction("Listing");
                    }
                    else
                    {
                        ViewBag.ErrorMsg = "Cylinder Type with same name already exists";
                        return View(obj);

                    }


                }
                else
                {
                    ViewBag.ErrorMsg = "Please fill mandatory fields";
                    return View(obj);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";
            }

            return RedirectToAction("Listing");

        }

        public ActionResult Edit(int id)
        {

            try
            {
                var CustomerMasters = repo.GetCylinderByID(id);
                CustomerMasters.amount = Convert.ToDecimal(string.Format("{0:0.00}", CustomerMasters.amount));
                return View(CustomerMasters);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";

            }

            return RedirectToAction("Listing");



        }


        [HttpPost]
        public ActionResult Edit(CylinderMaster obj)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    var cylindertypelist = repo.GetAllCylinders();
                    int count = cylindertypelist.Where(ite => ite.cylinderType.ToLower() == obj.cylinderType.ToLower()
                    && ite.ID != obj.ID).ToList().Count();

                    if (count <= 0)
                    {
                        obj.ModifiedBy = Convert.ToString(Session["UserName"]);
                        obj.ModifiedOn = System.DateTime.Now;
                        repo.Update(obj);
                        ViewBag.SuccessMsg = "Record Updated Successfully";
                        return View("Listing", cylindertypelist);
                    }
                    else
                    {
                        ViewBag.ErrorMsg = "Cylinder Type with same name already exists";
                        return View(obj);

                    }

                }
                else
                {
                    ViewBag.ErrorMsg = "Please fill mandatory fields";
                    return View(obj);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";
                return View(obj);
            }


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

            var CusotmerMasterlist = repo.GetAllCylinders();

            return View("Listing", CusotmerMasterlist);
        }


        public ActionResult Listing_LiveCylinder()
        {
            var CusotmerMasterlist = repo_Live.GetAllLiveCylinderDetail();


            return View(CusotmerMasterlist);
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
            var objCustomerlist = (from customer in repo.GetAllCylinders()
                                   where customer.cylinderType.StartsWith(prefix)
                                   select new
                                   {
                                       label = customer.cylinderType,
                                       val = customer.ID
                                   }).ToList();

            return Json(objCustomerlist);
        }
    }
}
