
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
    public class LoadController : Controller
    {
        honeysaleEntities db = new honeysaleEntities();
        LoadRepository repo = new LoadRepository(new honeysaleEntities());
        LoadCylinder lc = new LoadCylinder();

        public LoadController()
        {
            lc.users = repo.GetAllUsers();
        }

        public ActionResult Listing()
        {
            var LoadCylinderList = repo.GetAllLoadCylinders();

            return View(LoadCylinderList);
        }

        public ActionResult StaffCylinderDetail()
        {
            Staff s = new Staff();
            s.staffUnloadDetails = Enumerable.Empty<UnloadDetail>(); ;
            s.staffLoadDetails = Enumerable.Empty<LoadDetail>(); ;
            var staff = db.tblusers.Where(u => u.role_id == 2).Select(u => u.username).ToList();

            ViewBag.staffUsers = new SelectList(staff, "username");
            return View(s);
        }

        [HttpPost]
        public ActionResult StaffCylinderDetail(string FromDate, string Staff)
        {
            Staff s = new Staff();
            var staff = db.tblusers.Where(u => u.role_id == 2).Select(u => u.username).ToList();

            ViewBag.staffUsers = new SelectList(staff, "username");
            var fromdate = Convert.ToDateTime(FromDate);

            var Load_id = db.LoadCylinders.Where(a => a.userName == Staff && a.loadDate == fromdate).FirstOrDefault();
            var unLoad_id = db.UnloadCylinders.Where(a => a.userName == Staff && a.UnloadDate == fromdate).FirstOrDefault();
            if (unLoad_id != null)
            {
                s.staffUnloadDetails = db.UnloadDetails.Where(a => a.unload_Id == unLoad_id.ID).ToList();
            }
            else
            {

                s.staffUnloadDetails = Enumerable.Empty<UnloadDetail>();

            }
            if (Load_id != null)
            {
                s.staffLoadDetails = db.LoadDetails.Where(a => a.load_Id == Load_id.ID).ToList();
            }
            else
            {
                s.staffLoadDetails = Enumerable.Empty<LoadDetail>();
            }
            return View(s);
        }

        public ActionResult Create()
        {
            lc.users = repo.GetAllUsers();
            lc.LoadDetail = new List<LoadDetail> { new LoadDetail { } };
            lc.cylinders = repo.GetCylinderType();
            return View(lc);
        }

        [HttpPost]
        public JsonResult Create(LoadCylinder obj)
        {
            bool status = true;
            string vMessage = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (obj.LoadDetail != null)
                    {
                        foreach (var item in obj.LoadDetail)
                        {
                            var fiiledcount = db.LiveCylinderDetails.Where(ite => ite.cylinder_Id == item.cylinder_Id).FirstOrDefault().FilledCylinderCount;
                            if (fiiledcount <= 0)
                            {
                                status = false;
                                vMessage = "No Cylinder Available for " + item.cylinderType;
                                break;
                            }
                            else if (fiiledcount < item.FilledCylinder)
                            {
                                status = false;
                                vMessage = "Only " + fiiledcount + " filled cylinder available for " + item.cylinderType;
                                break;
                            }

                        }
                    }
                    if (status)
                    {
                        obj.createdBy = Convert.ToString(Session["UserName"]);
                        obj.createdOn = System.DateTime.Now;
                        obj.users = repo.GetAllUsers();
                        if (obj.LoadDetail != null)
                        {
                            repo.Insert(obj);
                        }
                        vMessage = "Record Added Successfully";
                        status = true;
                    }
                }
                else
                {
                    vMessage = "Please fill manadatory fields";
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

                var LoadCylinderList = repo.GetLoadCylinderByID(id);
                LoadCylinderList.LoadDetail = repo.GetLoadDetail(id);
                LoadCylinderList.cylinders = repo.GetCylinderType();
                LoadCylinderList.users = repo.GetAllUsers();
                LoadCylinderList.LoadDetailForEdit = new List<LoadDetail> { new LoadDetail { } };
                return View(LoadCylinderList);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";

            }

            return RedirectToAction("Listing");



        }


        [HttpPost]
        public JsonResult Edit(LoadCylinder obj)
        {
            bool status = true;
            string vMessage = "";
            try
            {
                if (ModelState.IsValid)
                {
                    if (obj.LoadDetail != null)
                    {
                        foreach (var item in obj.LoadDetail)
                        {
                            var fiiledcount = db.LiveCylinderDetails.Where(ite => ite.cylinder_Id == item.cylinder_Id).FirstOrDefault().FilledCylinderCount;
                            if (fiiledcount <= 0)
                            {
                                status = false;
                                vMessage = "No Cylinder Available for " + item.cylinderType;
                                break;
                            }
                            else if (fiiledcount < item.FilledCylinder)
                            {
                                status = false;
                                vMessage = "Only " + fiiledcount + " filled cylinder available for " + item.cylinderType;
                                break;
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
                    vMessage = "Please fill mandatory fields";
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

            var LoadCylinderList = repo.GetAllLoadCylinders();
            return View("Listing", LoadCylinderList);
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
        public JsonResult GetCustomerName(int? val)
        {
            var objCustomerlist = (from customer in repo.GetAllLoadCylinders()
                                   where customer.user_id.Equals(val)
                                   select new
                                   {
                                       label = customer.tbluser.firstname,
                                       val = customer.tbluser.ID
                                   }).ToList();

            return Json(objCustomerlist);
        }
    }
}
