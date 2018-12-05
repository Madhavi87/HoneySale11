
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoneySale.Models;
using HoneySaleDAL;
using Newtonsoft.Json;


namespace HoneySale.Controllers
{
    [SessionTimeout]
    public class PurchaseController : Controller
    {
        PurchaseRepository repo = new PurchaseRepository(new honeysaleEntities());
        PurchaseCylinder dd = new PurchaseCylinder();
        public ActionResult Listing()
        {
            var PurchaseMasterlist = repo.GetAllPurchaseCylinder();

            return View(PurchaseMasterlist);
        }

        public ActionResult Create()
        {
            dd.cylinders = repo.GetCylinderType();
            dd.PurchaseDetail = new List<PurchaseDetail> { new PurchaseDetail { } };
            return View(dd);
        }

        [HttpPost]
        public JsonResult Create(PurchaseCylinder obj)
        {
            bool status = false;
            string vMessage = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (obj.PurchaseDetail != null)
                    {

                        PurchaseCylinder purchase = new PurchaseCylinder();
                        if (obj.PurchaseDetail != null)
                        {
                            repo.Insert(obj);
                        }
                        vMessage = "Record Added Successfully";
                        status = true;
                    }
                }
                else
                {
                    vMessage = "Please fill mandatory fields";
                }
            }
            catch (Exception ex)
            {
                //obj.cylinders = repo.GetCylinderType();
                //obj.PurchaseDetail = new List<PurchaseDetail> { new PurchaseDetail { } };
                vMessage = "Sorry Some Problem Occured";
            }
            return new JsonResult { Data = new { status = status, Message = vMessage } };


        }

        public ActionResult Edit(int id)
        {

            try
            {
                var PurchaseMaster = repo.GetPurchaseByID(id);
                PurchaseMaster.cylinders = repo.GetCylinderType();
                PurchaseMaster.PurchaseDetail = repo.GetPurchaseDetail(id);
                PurchaseMaster.PurchaseDetailForEdit = new List<PurchaseDetail> { new PurchaseDetail { } };
                return View(PurchaseMaster);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";

            }

            return RedirectToAction("Listing");

        }


        [HttpPost]
        public ActionResult Edit(PurchaseCylinder obj)
        {
            bool status = false;
            try
            {
                if (ModelState.IsValid)
                {

                    obj.cylinders = repo.GetCylinderType();
                    repo.Update(obj);
                    ViewBag.SuccessMsg = "Record Updated Successfully";
                    status = true;
                }
                else
                {
                    ViewBag.ErrorMsg = "Please fill mandatory fields";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";
            }
            return new JsonResult { Data = new { status = status } };

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

            var PurchaseMasterList = repo.GetAllPurchaseCylinder();
            return View("Listing", PurchaseMasterList);
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


        [HttpPost]
        public ActionResult GetAmount(int? val)
        {
            if (val != null)
            {
                CylinderMaster c = new CylinderMaster();
                c = repo.GetCylinderType().Where(obj => obj.ID == val).FirstOrDefault();

                return Json(new { Success = "true", Data = new { amount = c.amount, ctype = c.cylinderType } });
            }
            else
                return Json(new { Success = "false" });

        }

        [HttpPost]
        public ActionResult GetPurchaseDetail(int? val)
        {
            if (val != null)
            {
                int id = Convert.ToInt32(val);
                var PurchaseDetail = repo.GetPurchaseDetail(id).ToList();
                foreach (var c in PurchaseDetail)
                {
                    //  Data = new { cylinderType = c.cylinderType }
                }
                return Json(new { Success = "true" });
            }
            else
                return Json(new { Success = "false" });

        }

        [HttpPost]
        public JsonResult GetPurchaseName(string prefix)
        {
            var objCustomerlist = (from customer in repo.GetAllPurchaseCylinder()
                                   where customer.dealerName.StartsWith(prefix)
                                   select new
                                   {
                                       label = customer.dealerName,
                                       val = customer.ID
                                   }).ToList();

            return Json(objCustomerlist);
        }
    }
}
