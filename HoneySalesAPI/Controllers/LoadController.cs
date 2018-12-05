using HoneySaleDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace HoneySalesAPI.Controllers
{
    // [Authorize]
    public class LoadController : ApiController
    {
        LoadRepository repo = new LoadRepository(new honeysaleEntities());
        honeysaleEntities db = new honeysaleEntities();
        // GET api/values

        public JsonResult GetAllStaffUsers()
        {
            //List<tbluser> objUserList = new List<tbluser>();
            var status = true;
            var objCustomerlist = db.tblusers.Where(item => item.role_id == 2 && item.isActive)
                           .Select(c => new { Name = c.username, ID = c.ID })
              .Distinct().OrderBy(ite => ite.Name).ToList();

            // try
            // {

            // }
            // catch (Exception ex)
            // {
            //  status = false;

            // }

            return new System.Web.Mvc.JsonResult()
            {
                Data = new
                {
                    status = true,
                    result = objCustomerlist
                }
            };

        }


        public JsonResult SaveLoadCylinderDetails(int userid, DateTime loadDate,
            int cylindertype, int nooffilledcylinder, string remark)
        {
            string vErroMessage = "";
            bool status = true;
            try
            {
                //if (obj.LoadDetail != null)
                //{
                //    foreach (var item in obj.LoadDetail)
                //    {
                var fiiledcount = db.LiveCylinderDetails.Where(ite => ite.cylinder_Id == cylindertype).FirstOrDefault().FilledCylinderCount;
                if (fiiledcount <= 0)
                {
                    status = false;
                    vErroMessage = "No Cylinder Available";

                }
                else if (fiiledcount < nooffilledcylinder)
                {
                    status = false;
                    vErroMessage = "Only " + fiiledcount + " filled cylinder available";

                }

                //    }
                //}
                //if (db.LiveCylinderDetails.Where(ite => ite.cylinder_Id == cylindertype).FirstOrDefault().FilledCylinderCount <= 0)
                //{
                //    status = false;
                //    vErroMessage = "No Cylinder Available";
                //}
                if (status)
                {
                    LoadCylinder obj = new LoadCylinder();
                    obj.createdBy = User.Identity.Name;
                    obj.createdOn = System.DateTime.Now;
                    obj.user_id = userid;

                    string vUsername = db.tblusers.Where(obj1 => (obj1.ID == userid)).SingleOrDefault().username;
                    obj.loadDate = loadDate;
                    List<LoadDetail> objdlist = new List<LoadDetail>();
                    LoadDetail objd = new LoadDetail();
                    obj.userName = vUsername;
                    objd.FilledCylinder = nooffilledcylinder;
                    objd.cylinder_Id = cylindertype;

                    string cylindertypename = db.CylinderMasters.Where(ite => ite.ID == cylindertype).FirstOrDefault().cylinderType;
                    objd.cylinderType = cylindertypename;
                    objdlist.Add(objd);

                    obj.LoadDetail = objdlist;
                    repo.Insert(obj);

                    vErroMessage = "Record Added Successfully";
                    status = true;
                }
            }
            catch (Exception ex)
            {
                vErroMessage = "Error Occured while your transaction!";
                status = false;
            }

            return new JsonResult()
            {
                Data = new
                {
                    status = status,
                    Message = vErroMessage
                }
            };

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyName"></param>
        /// <param name="address"></param>
        /// <param name="contact_Num_1"></param>
        /// <param name="email"></param>
        /// <param name="discount"></param>
        /// <returns></returns>
        public JsonResult SaveCustomerDetails(string companyName, string address,
          string contact_Num_1, string email, decimal discount)
        {

            string vErroMessage = "";
            bool status = false;
            try
            {
                vErroMessage = "Record Added Successfully";
                status = true;
            }
            catch (Exception ex)
            {
                vErroMessage = "Error Occured while your transaction!";
                status = false;
            }

            return new JsonResult()
            {
                Data = new
                {
                    status = status,
                    Message = vErroMessage
                }
            };

        }


        public JsonResult SaveUnLoadCylinderDetails(int userid, DateTime unloadDate,
                   int cylindertype, int nooffilledcylinder, int noofemptycylinder, int noofrejectedcylinder, string remark)
        {

            string vErroMessage = "";
            bool status = false;
            try
            {

                UnloadCylinder obj = new UnloadCylinder();
                obj.createdBy = User.Identity.Name;
                obj.createdOn = System.DateTime.Now;
                obj.user_id = userid;
                string vUsername = db.tblusers.Where(obj1 => (obj1.ID == userid)).SingleOrDefault().username;
                obj.UnloadDate = unloadDate;
                obj.userName = vUsername;
                List<UnloadDetail> objdlist = new List<UnloadDetail>();
                UnloadDetail objd = new UnloadDetail();
                objd.FilledCylinder = nooffilledcylinder;
                objd.EmptyCylinder = noofemptycylinder;
                objd.RejectedCylinder = noofrejectedcylinder;
                objd.cylinder_Id = cylindertype;
                string cylindertypename = db.CylinderMasters.Where(ite => ite.ID == cylindertype).FirstOrDefault().cylinderType;
                objd.cylinderType = cylindertypename;
                objdlist.Add(objd);


                vErroMessage = "Record Added Successfully";
                status = true;
            }
            catch (Exception ex)
            {
                vErroMessage = "Error Occured while your transaction!";
                status = false;
            }

            return new JsonResult()
            {
                Data = new
                {
                    status = status,
                    Message = vErroMessage
                }
            };

        }



    }
}