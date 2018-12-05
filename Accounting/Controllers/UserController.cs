
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HoneySaleDAL;
using HoneySale.Models;
using System.Text;
using System.Security.Cryptography;


namespace HoneySale.Controllers
{

    [SessionTimeout]
    public class UserController : Controller
    {
        honeysaleEntities db = new honeysaleEntities();
        UserRegistrationVM vm = new UserRegistrationVM();
        UserRepository repo1 = new UserRepository(new honeysaleEntities());
        tbluser t = new tbluser();
        public ActionResult Listing()
        {
            var UserMasterlist = (from cont in repo1.GetAllUsers()
                                  join rol in db.tblroles on cont.role_id equals rol.roleid
                                  select new tbluser()
                                  {
                                      ID = cont.ID,
                                      firstname = cont.firstname,
                                      lastname = cont.lastname,
                                      username = cont.username,
                                      passwordHash = cont.passwordHash,
                                      phone = cont.phone,
                                      role_id = cont.role_id,
                                      RoleName = rol.rolename
                                  }
                                  ).ToList();
            return View(UserMasterlist);
        }

        public ActionResult Create()
        {

            t.roles = repo1.GetAllRole();
            return View(t);
        }

        [HttpPost]
        public ActionResult Create(tbluser obj)
        {
            var CustomerMasterlist = repo1.GetAllUsers();

            try
            {

                if (ModelState.IsValid)
                {
                    if (CustomerMasterlist.Where(ite => ite.username.ToLower() == obj.username.ToLower()).FirstOrDefault() != null)
                    {

                        ViewBag.ErrorMsg = "User already exists";
                        t.roles = repo1.GetAllRole();
                        return View(t);
                    }
                    else
                    {
                        obj.passwordHash = vm.Encrypt(obj.passwordHash);
                        repo1.Insert(obj);
                        ViewBag.SuccessMsg = "Record Added Successfully";
                        CustomerMasterlist = repo1.GetAllUsers();
                        return RedirectToAction("Listing");
                    }
                }
                else
                {
                    t.roles = repo1.GetAllRole();
                    return View(t);
                }
            }
            catch (Exception ex)
            {

                ViewBag.ErrorMsg = "Sorry Some Problem Occured" + ex.ToString();
            }

            return RedirectToAction("Listing");

        }

        public ActionResult Edit(int id)
        {
            try
            {
                var UserMasters = repo1.GetUserByID(id);
                UserMasters.passwordHash = vm.Decrypt(UserMasters.passwordHash);
                int term = Convert.ToInt32(UserMasters.role_id);
                UserMasters.roles = repo1.GetAllRole();
                return View(UserMasters);
            }
            catch (Exception)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";

            }

            return RedirectToAction("Listing");

        }


        [HttpPost]
        public ActionResult Edit(tbluser obj)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var CustomerMasterlist = repo1.GetAllUsers();
                    if (CustomerMasterlist.Where(ite => ite.username.ToLower() == obj.username.ToLower()).FirstOrDefault() != null)
                    { 
                        ViewBag.ErrorMsg = "User already exists"; 
                        return View(obj);
                    }
                    obj.passwordHash = vm.Encrypt(obj.passwordHash);
                    repo1.Update(obj);
                    ViewBag.SuccessMsg = "Record Updated Successfully";
                }
                else
                {
                    return View(obj);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";
            }

            return RedirectToAction("Listing");
        }

        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                //   repo1.Delete(id);
                repo1.Delete(id);
                ViewBag.SuccessMsg = "Record Deleted Successfully";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";
            }

            return RedirectToAction("Listing");
        }

        public JsonResult GetUserName(string term = "")
        {
            var objCustomerlist = repo1.GetAllUsers()
                            .Where(c => c.username.ToUpper()
                            .Contains(term.ToUpper()))
                            .Select(c => new { Name = c.firstname, ID = c.ID })
                            .Distinct().ToList();
            return Json(objCustomerlist, JsonRequestBehavior.AllowGet);
        }
    }
}
