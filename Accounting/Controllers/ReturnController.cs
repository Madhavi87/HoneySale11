
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
    public class ReturnController : Controller
    {
        honeysaleEntities db = new honeysaleEntities();
        ReturnRepository repo = new ReturnRepository(new honeysaleEntities());
        ReturnCylinder lc = new ReturnCylinder();


        public ActionResult Listing()
        {
            var ReturnMasterList = repo.GetAllReturns();

            return View(ReturnMasterList);
        }

        public ActionResult Create()
        {
            lc.ReturnDetail = new List<ReturnDetail> { new ReturnDetail { } };
            lc.cylinders = repo.GetCylinderType();
            return View(lc);
        }

        [HttpPost]
        public JsonResult Create(ReturnCylinder obj)
        {
            bool status = true;
            string vMessage = "";
            try
            {
                if (ModelState.IsValid)
                {

                    if (obj.ReturnDetail != null)
                    {
                        foreach (var item in obj.ReturnDetail)
                        {
                            if (db.LiveCylinderDetails.Where(ite => ite.cylinder_Id == item.cylinder_Id).FirstOrDefault().EmptyCylinderCount <= 0)
                            {
                                vMessage = "No Cylinder Available";
                                status = false;
                                break;
                            }

                            var fiiledcount = db.LiveCylinderDetails.Where(ite => ite.cylinder_Id == item.cylinder_Id).FirstOrDefault();
                            if (fiiledcount.EmptyCylinderCount <= 0 && item.EmptyCylinder > 0)
                            {
                                status = false;
                                vMessage = "No empty Cylinder Available for " + item.cylinderType;
                                break;
                            }
                            else if (fiiledcount.FilledCylinderCount <= 0 && item.FilledCylinder > 0)
                            {
                                status = false;
                                vMessage = "No filled Cylinder Available for " + item.cylinderType;
                                break;
                            }
                            else if (fiiledcount.ReplacementCylinderCount <= 0 && item.RejectedCylinder > 0)
                            {
                                status = false;
                                vMessage = "No rejected Cylinder Available for " + item.cylinderType;
                                break;
                            }
                            else if (fiiledcount.EmptyCylinderCount < item.EmptyCylinder && item.EmptyCylinder > 0)
                            {
                                status = false;
                                vMessage = "Only " + fiiledcount.EmptyCylinderCount + " empty cylinder available for " + item.cylinderType;
                                break;
                            }
                            else if (fiiledcount.FilledCylinderCount < item.FilledCylinder && item.FilledCylinder > 0)
                            {
                                status = false;
                                vMessage = "Only " + fiiledcount.FilledCylinderCount + " empty cylinder available for " + item.cylinderType;
                                break;
                            }

                        }

                    }
                    if (status)
                    {
                        if (obj.ReturnDetail != null)
                        {
                            repo.Insert(obj);
                        }
                        vMessage = "Record Added Successfully";
                        status = true;
                    }
                }
                else
                {
                    status = false;
                    vMessage = "Please fill mandatory fields";
                }

            }
            catch (Exception ex)
            {
                vMessage = "Sorry Some Problem Occured";
                status = false;
            }

            return new JsonResult { Data = new { status = status, Message = vMessage } };

        }

        public ActionResult Edit(int id)
        {

            try
            {

                var ReturnMasterList = repo.GetReturnByID(id);
                ReturnMasterList.cylinders = repo.GetCylinderType();
                ReturnMasterList.ReturnDetail = repo.GetReturnDetail(id);
                ReturnMasterList.ReturnDetailForEdit = new List<ReturnDetail> { new ReturnDetail { } };

                return View(ReturnMasterList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";

            }

            return RedirectToAction("Listing");
        }


        [HttpPost]
        public JsonResult Edit(ReturnCylinder obj)
        {
            bool status = true;
            string vMessage = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (obj.ReturnDetail != null)
                    {
                        foreach (var item in obj.ReturnDetail)
                        {
                            if (db.LiveCylinderDetails.Where(ite => ite.cylinder_Id == item.cylinder_Id).FirstOrDefault().EmptyCylinderCount <= 0)
                            {
                                vMessage = "No Cylinder Available";
                                status = false;
                                break;
                            }

                        }

                    }

                    if (status)
                    {
                        repo.Update(obj);
                        vMessage = "Record Updated Successfully";
                        status = true;
                    }
                }
                else
                {
                    status = false;

                }
            }
            catch (Exception ex)
            {
                vMessage = "Sorry Some Problem Occured";
                status = false;
            }

            return new JsonResult { Data = new { status = status, Message = vMessage } };

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

            var ReturnMasterList = repo.GetAllReturns();
            return View("Listing", ReturnMasterList);
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
        public ActionResult DeleteDetail(int? val)
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
        public JsonResult GetReturnName(string prefix)
        {
            var objReturnlist = (from customer in repo.GetAllReturns()
                                 where customer.dealerName.StartsWith(prefix)
                                 select new
                                 {
                                     label = customer.dealerName,
                                     val = customer.ID
                                 }).ToList();

            return Json(objReturnlist);
        }
    }
}
