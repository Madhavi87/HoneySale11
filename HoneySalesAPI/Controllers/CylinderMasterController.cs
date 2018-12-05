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
    public class CylinderMasterController : ApiController
    {
        honeysaleEntities _db = new honeysaleEntities();
        // GET api/values
        public JsonResult GetCylinderMasterDetails()
        {

          
            //var data = new List<CylinderMaster>();
            //string vErroMessage = "";
            //bool status = false;
            //try
            //{
            //    data = _db.CylinderMasters.ToList();
            //    status = true;
            //}
            //catch (Exception ex)
            //{
            //    vErroMessage = ex.Message;
            //    status = false;
            //    data = new List<CylinderMaster>();
            //}
            var cylinderlist = _db.CylinderMasters
                                .Select(c => new { cylinderType = c.cylinderType, ID = c.ID })
                  .Distinct().ToList();


            return new JsonResult()
            {
                Data = new
                {
                    Data = cylinderlist,
                    status = true,
                    Message = ""
                }
            };

        }


        // GET api/values
        public JsonResult GetCylinderMasterListbyUser(int userid)
        {
            //var data = new List<CylinderMaster>();
            //string vErroMessage = "";
            //bool status = false;
            //try
            //{
            //    data = _db.CylinderMasters.ToList();
            //    status = true;
            //}
            //catch (Exception ex)
            //{
            //    vErroMessage = ex.Message;
            var status = true;
            //    data = new List<CylinderMaster>();
            //}

            var cylinderlist = _db.CylinderMasters
                               .Select(c => new { cylinderType = c.cylinderType, ID = c.ID })
                 .Distinct().ToList();


            return new JsonResult()
            {
                Data = new
                {
                    Data = cylinderlist,
                    status = status,
                    Message = ""
                }
            };

        }


    }
}