using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using HoneySale.Models;
using HoneySaleDAL;
using System.Text;
using System.Security.Cryptography;
using System.Data.Entity;
using System.Data.Objects;

namespace HoneySale.Controllers
{
    public class LoginController : Controller
    {
        UserRegistrationVM vm = new UserRegistrationVM();
        honeysaleEntities _db = new honeysaleEntities();
        
        public ActionResult Login()
        {
            tbluser u = new tbluser();
           
           // tbluser objUserRegistrationVM = new tbluser();
            UserRegistrationVM objUserRegistrationVM = new UserRegistrationVM();
            return View(objUserRegistrationVM);
        }


        [HttpPost]
        public ActionResult Login(UserRegistrationVM UR)
        {
         
            if (string.IsNullOrEmpty(UR.UserName) || string.IsNullOrEmpty(UR.UserName))
            {
                ViewBag.ErrorMessage = "Please fill mandatory fields";
                return View();
            }
                        
            var password=UR.Encrypt(UR.Password);
          
                var output = _db.tblusers.FirstOrDefault(obj => obj.username == UR.UserName  && obj.passwordHash == password && obj.role_id.Value == 1);

            if (output != null)
                {
                    FormsAuthentication.SetAuthCookie(UR.UserName, true);
                    Session["UserName"] = UR.UserName.ToUpper();
                    return RedirectToAction("Home", "Home");
                }
            
            else
            {
                ViewBag.ErrorMessage = "Invalid Credentials!";
                return View();
            }

           

        }


        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            UserRegistrationVM objUserRegistrationVM = new UserRegistrationVM();
            //tbluser objUserRegistrationVM = new tbluser();
            return View("Login", objUserRegistrationVM);
        }



    }
}
