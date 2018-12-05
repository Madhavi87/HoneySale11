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
    //[Authorize]


    public class UnLoadController : ApiController
    {
        UnloadRepository unrepo = new UnloadRepository(new honeysaleEntities());
        LoadRepository repo = new LoadRepository(new honeysaleEntities());
        honeysaleEntities db = new honeysaleEntities();
        public JsonResult SaveUnLoadCylinderDetails(int userid, DateTime unloadDate,
                    int cylindertype, int nooffilledcylinder, int noofemptycylinder, int noofrejectedcylinder, string remark)
        {

            string vErroMessage = "";
            bool status = true;
            try
            {

                var deliverycount = (
                            from delivery in db.DeliveryDetails
                            where delivery.userid == userid && delivery.C_deliveryDate == unloadDate
                            && delivery.unload_Flag == false
                            select delivery
                            ).ToList();

                if (deliverycount == null || deliverycount.Count <= 0)
                {
                    status = false;
                    vErroMessage = "No Delivery found " + "on" + unloadDate;
                }
                else
                {

                    // foreach (var item in obj.UnloadDetail)
                    // {
                    var deliverlist = deliverycount.Where(ite => ite.cylinder_Id == cylindertype).ToList();
                    var existingemptycount = deliverlist.Sum(ite => ite.emptyCylinder).HasValue ? deliverlist.Sum(ite => ite.emptyCylinder).Value : 0;
                    if (noofemptycylinder != existingemptycount)
                    {
                        status = false;
                        vErroMessage = "Empty Cylinder counts are not matching with delivery details";

                    }
                    var existingreplacecount = deliverlist.Sum(ite => ite.emptyCylinder).HasValue ? deliverlist.Sum(ite => ite.emptyCylinder).Value : 0;
                    if (noofrejectedcylinder != existingreplacecount)
                    {
                        status = false;
                        vErroMessage = "Replacement Cylinder counts are not matching with delivery details";

                    }
                }


                if (status)
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
                    obj.UnloadDetail = objdlist;
                    unrepo.Insert(obj);

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
    }
}
