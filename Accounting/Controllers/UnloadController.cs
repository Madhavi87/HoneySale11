
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
    public class UnloadController : Controller
    {
        UnloadRepository repo = new UnloadRepository(new honeysaleEntities());
        UnloadCylinder lc = new UnloadCylinder();
        honeysaleEntities db = new honeysaleEntities();
        public UnloadController()
        {
            lc.users = repo.GetAllUsers();
        }

        public ActionResult Listing()
        {
            var UnloadCylinderList = repo.GetAllUnloadCylinders();

            return View(UnloadCylinderList);
        }

        public ActionResult Create()
        {
            lc.users = repo.GetAllUsers();
            lc.cylinders = repo.GetCylinderType();
            lc.UnloadDetail = new List<UnloadDetail> { new UnloadDetail { } };
            return View(lc);
        }

        [HttpPost]
        public JsonResult Create(UnloadCylinder obj)
        {
            bool status = true;
            string vMessage = "";
            try
            {


                if (ModelState.IsValid)
                {
                    if (obj.UnloadDetail != null)
                    {
                        //foreach (var item in obj.UnloadDetail)
                        //{
                        var deliverycount = (
                            from delivery in db.DeliveryDetails
                            where delivery.userid == obj.user_id && delivery.C_deliveryDate == obj.UnloadDate
                            && delivery.unload_Flag == false
                            select delivery
                            ).ToList();

                        if (deliverycount == null || deliverycount.Count <= 0)
                        {
                            status = false;
                            vMessage = "No Delivery found " + "on" + Models.CommonFunction.GetShortDayFormat(obj.UnloadDate);
                        }
                        else
                        {

                            foreach (var item in obj.UnloadDetail)
                            {
                                var deliverlist = deliverycount.Where(ite => ite.cylinder_Id == item.cylinder_Id).ToList();
                                var existingemptycount = deliverlist.Sum(ite => ite.emptyCylinder).HasValue ? deliverlist.Sum(ite => ite.emptyCylinder).Value : 0;
                                if (item.EmptyCylinder != existingemptycount)
                                {
                                    status = false;
                                    vMessage = "Empty Cylinder counts are not matching with delivery details";
                                    break;
                                }
                                var existingreplacecount = deliverlist.Sum(ite => ite.emptyCylinder).HasValue ? deliverlist.Sum(ite => ite.emptyCylinder).Value : 0;
                                if (item.RejectedCylinder != existingreplacecount)
                                {
                                    status = false;
                                    vMessage = "Replacement Cylinder counts are not matching with delivery details";
                                    break;
                                }
                            }
                        }


                        //var fiiledcount = (
                        //    from load in db.LoadCylinders
                        //    join loaddetail in db.LoadDetails on load.ID equals loaddetail.load_Id
                        //    where load.user_id == obj.user_id && load.loadDate == obj.UnloadDate
                        //    && loaddetail.cylinder_Id == item.cylinder_Id
                        //    select loaddetail
                        //    ).FirstOrDefault();
                        //if (fiiledcount != null)
                        //{

                        //var cylindercount = (from del in db.DeliveryDetails
                        //                     where del.C_deliveryDate == obj.UnloadDate
                        //                   //  && del.userid == obj.user_id
                        //                   && del.cylinder_Id == item.cylinder_Id
                        //                     select del).FirstOrDefault();

                        //if (cylindercount.emptyCylinder != item.EmptyCylinder)
                        //{
                        //    status = false;
                        //    vMessage = "User not delivered any cylinder for" + item.cylinderType + "on" + Models.CommonFunction.GetShortDayFormat(obj.UnloadDate);
                        //    break;

                        //}

                        //}
                        //else
                        //{
                        //    status = false;
                        //    vMessage = "No Cylinder available for unload for cylinder" + item.cylinderType + "for" + Models.CommonFunction.GetShortDayFormat(obj.UnloadDate);
                        //    break;
                        //}

                        //  }
                    }


                    if (status)
                    {

                        obj.createdBy = Convert.ToString(Session["UserName"]);
                        obj.createdOn = System.DateTime.Now;
                        obj.users = repo.GetAllUsers();
                        if (obj.UnloadDetail != null)
                        {
                            repo.Insert(obj);
                            vMessage = "Record Added Successfully";
                            var UnloadCylinderList = repo.GetAllUnloadCylinders();
                            status = true;
                        }

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
            }

            return new JsonResult { Data = new { status = status, Message = vMessage } };

        }

        public ActionResult Edit(int id)
        {

            try
            {

                var UnloadCylinderList = repo.GetUnloadCylinderByID(id);
                UnloadCylinderList.users = repo.GetAllUsers();
                UnloadCylinderList.UnloadDetail = repo.GetUnloadDetail(id);
                UnloadCylinderList.cylinders = repo.GetCylinderType();
                UnloadCylinderList.UnloadDetailForEdit = new List<UnloadDetail> { new UnloadDetail { } };
                return View(UnloadCylinderList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";

            }

            return RedirectToAction("Listing");

        }


        [HttpPost]
        public JsonResult Edit(UnloadCylinder obj)
        {
            bool status = false;
            string vMessage = "";
            try
            {

                if (ModelState.IsValid)
                {

                    if (obj.UnloadDetail != null)
                    {
                        foreach (var item in obj.UnloadDetail)
                        {
                            //var fiiledcount = (
                            //    from load in db.LoadCylinders
                            //    join loaddetail in db.LoadDetails on load.ID equals loaddetail.load_Id
                            //    where load.user_id == obj.user_id && load.loadDate == obj.UnloadDate
                            //    && loaddetail.cylinder_Id == item.cylinder_Id
                            //    select loaddetail
                            //    ).FirstOrDefault();
                            //if (fiiledcount == null)
                            //{
                            //    status = false;
                            //    vMessage = "No Cylinder available for unload for cylinder" + item.cylinderType + "for" + Models.CommonFunction.GetShortDayFormat(obj.UnloadDate);
                            //    break;
                            //}
                            //else
                            {
                                var cylindercount = (from del in db.DeliveryDetails
                                                     where del.C_deliveryDate == obj.UnloadDate
                                                   //  && del.userid == obj.user_id
                                                   && del.cylinder_Id == item.cylinder_Id
                                                     select del).FirstOrDefault();

                                if (cylindercount.emptyCylinder != item.EmptyCylinder)
                                {
                                    status = false;
                                    vMessage = "User not delivered any cylinder for" + item.cylinderType + "on" + Models.CommonFunction.GetShortDayFormat(obj.UnloadDate);
                                    break;

                                }
                            }

                        }
                    }




                    if (status)
                    {

                        obj.users = repo.GetAllUsers();
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

            var UnloadCylinderList = repo.GetAllUnloadCylinders();
            return View("Listing", UnloadCylinderList);
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
        public ActionResult GetUnloadDetail(int? val)
        {
            if (val != null)
            {
                int id = Convert.ToInt32(val);
                var purchaseDetail = repo.GetUnloadDetail(id).ToList();
                foreach (var c in purchaseDetail)
                {
                    //  Data = new { cylinderType = c.cylinderType }
                }
                return Json(new { Success = "true" });
            }
            else
                return Json(new { Success = "false" });

        }

        [HttpPost]
        public JsonResult GetUnloadCylinder(int? prefix)
        {
            var objCustomerlist = (from unload in repo.GetAllUnloadCylinders()
                                   where unload.user_id.Equals(prefix)
                                   select new
                                   {
                                       label = unload.tbluser.username,
                                       val = unload.user_id
                                   }).ToList();

            return Json(objCustomerlist);
        }
    }
}
