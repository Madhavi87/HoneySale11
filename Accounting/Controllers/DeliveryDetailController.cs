
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using HoneySaleDAL;
using System.Threading.Tasks;
using Num2Wrd;
using System.IO;
using HoneySale.Models;
using ClosedXML.Excel;
using System.Data;

namespace HoneySale.Controllers
{
    [SessionTimeout]
    public class DeliveryDetailController : Controller
    {
        honeysaleEntities db = new honeysaleEntities();
        DeliveryDetailRepository repo = new DeliveryDetailRepository(new honeysaleEntities());
        DeliveryDetail dd = new DeliveryDetail();


        public ActionResult Listing()
        {
            var DeliveryDetaillist = repo.GetAllDeliveryDetails();

            return View(DeliveryDetaillist);
        }

        public ActionResult DeliveryDetailReport()
        {
            var DeliveryDetaillist = repo.GetAllDeliveryDetails();
            var customers = repo.GetAllCustomers().Select(a => a.companyName).ToList();
            ViewBag.CustomerNames = new SelectList(customers, "companyName");

            return View(DeliveryDetaillist);
        }

        [HttpPost]
        public ActionResult DeliveryDetailReport(string FromDate, string ToDate, string CustomerNames)
        {
            var customers = repo.GetAllCustomers().Select(a => a.companyName).ToList();
            ViewBag.CustomerNames = new SelectList(customers, "companyName");


            if (FromDate != "" && ToDate != "" && CustomerNames != "")
            {
                var from = Convert.ToDateTime(FromDate);
                var to = Convert.ToDateTime(ToDate);
                var DeliveryDetaillist = db.DeliveryDetails.Where(a => a.C_deliveryDate >= from && a.C_deliveryDate <= to && a.CustomerDetail.companyName == CustomerNames);
                Session["DeliveryDetaillist"] = DeliveryDetaillist.ToList();
                return View(DeliveryDetaillist);
            }
            else if (FromDate != "" && ToDate != "" || CustomerNames == "")
            {
                var from = Convert.ToDateTime(FromDate);
                var to = Convert.ToDateTime(ToDate);
                var DeliveryDetaillist = db.DeliveryDetails.Where(a => a.C_deliveryDate >= from && a.C_deliveryDate <= to);
                Session["DeliveryDetaillist"] = DeliveryDetaillist.ToList();
                return View(DeliveryDetaillist);
            }
            else
            {
                if (CustomerNames != "" && (FromDate == "" || ToDate == ""))
                {
                    var DeliveryDetaillist = db.DeliveryDetails.Where(a => a.CustomerDetail.companyName == CustomerNames);
                    Session["DeliveryDetaillist"] = DeliveryDetaillist.ToList();
                    return View(DeliveryDetaillist);

                }
            }
            Session["DeliveryDetaillist"] = null;
            return View();

        }


        public ActionResult Create()
        {
            try
            {
                dd.invoiceCheck = true;
                int count = db.DeliveryDetails.Count();
                if (count == 0)
                {
                    ViewBag.VoucherNo = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["voucherNo"]);
                }
                else
                {
                    var lastvoucherNo = db.DeliveryDetails.Where(o => o.voucherNo != "LPG-E000").OrderByDescending(o => o.voucherNo).Select(m => m.voucherNo).FirstOrDefault();
                    lastvoucherNo = lastvoucherNo.Replace("LPG-", "");
                    int last = Convert.ToInt32(lastvoucherNo);

                    ViewBag.VoucherNo = "LPG-" + (++last).ToString("D" + lastvoucherNo.Length);
                }
                //dd.cylinders = repo.GetCylinderType();
                dd.users = repo.GetAllUsers();
                dd.customers = repo.GetAllCustomers();

                return View(dd);
            }
            catch (Exception)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";

            }

            return RedirectToAction("Listing");
        }

        [HttpPost]
        public JsonResult Create(DeliveryDetail obj)
        {
            bool status = true;
            string vMessage = "";



            var result = (from cont in db.LoadCylinders
                          join det in db.LoadDetails on cont.ID equals det.load_Id
                          where cont.loadDate == obj.C_deliveryDate && cont.user_id == obj.userid
                          && det.cylinder_Id == obj.cylinder_Id
                          select det).ToList();

            int loadcount = 0;
            if (result != null)
                loadcount = result.Sum(ite => ite.FilledCylinder).Value;

            var DeliveryDetaillist = repo.GetAllDeliveryDetails();
            var deliverydetaillist = (from del in db.DeliveryDetails
                                      where del.C_deliveryDate == obj.C_deliveryDate &&
                                      del.userid == obj.userid
                                      select del).ToList();

            int deliverydetailcount = 0;
            if (deliverydetaillist != null)
                deliverydetailcount = deliverydetaillist.Sum(ite => ite.filledCylinder).Value + obj.filledCylinder.Value;
            else
                deliverydetailcount = obj.filledCylinder.Value;

            if (result == null || deliverydetailcount > loadcount)
            {
                status = false;
            }
            else
                status = true;




            if (status)
            {
                obj.customers = repo.GetAllCustomers();
                obj.users = repo.GetAllUsers();
                try
                {
                    int count = db.DeliveryDetails.Count();
                    if (count == 0)
                    {
                        ViewBag.VoucherNo = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["voucherNo"]);
                    }
                    else
                    {
                        var lastvoucherNo = db.DeliveryDetails.Where(o => o.voucherNo != "LPG-E000").OrderByDescending(o => o.voucherNo).Select(m => m.voucherNo).FirstOrDefault();
                        lastvoucherNo = lastvoucherNo.Replace("LPG-", "");
                        int last = Convert.ToInt32(lastvoucherNo);

                        ViewBag.VoucherNo = "LPG-" + (++last).ToString("D" + lastvoucherNo.Length);
                    }


                    if (ModelState.IsValid)
                    {
                        var gst = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["gst"]);
                        var total = Convert.ToDecimal(obj.totalAmount) / gst;
                        var diff = Convert.ToDecimal(obj.totalAmount) - total;
                        obj.cgst = diff / 2;
                        obj.sgst = diff / 2;
                       // obj.unload_Flag = true;
                        int id = repo.Insert(obj);
                        status = true;
                        vMessage = "Record Added Successfully";

                        if (id > 0)
                            SendEmail(id);
                    }
                    else
                    {
                        status = false;
                        vMessage = "Please enter valid data";
                    }
                }
                catch (Exception ex)
                {
                    status = false;
                    vMessage = "Sorry Some Problem Occured";

                }
            }
            else
            {
                status = false;
                vMessage = "Cylider Not available";

            }
            return new JsonResult { Data = new { status = status, Message = vMessage } };

        }

        public ActionResult Edit(int id)
        {

            try
            {

                var DeliveryDetails = repo.GetDeliveryDetailByID(id);
                var c = new List<CylinderMaster>();
                var objCylinderlist = db.CustomerCylinderDetails.Where(a => a.cust_id == DeliveryDetails.cust_id).ToList();
                foreach (var t in objCylinderlist)
                {
                    CylinderMaster temp = db.CylinderMasters.Where(a => a.cylinderType == t.cylinderType).FirstOrDefault();
                    c.Add(temp);
                }
                DeliveryDetails.cylinders = c;
                DeliveryDetails.users = repo.GetAllUsers();
                DeliveryDetails.customers = repo.GetAllCustomers();
                TempData["TotalAmount"] = DeliveryDetails.totalAmount;
                return View(DeliveryDetails);
            }
            catch (Exception)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";

            }
            return RedirectToAction("Listing");

        }


        [HttpPost]
        public ActionResult Edit(DeliveryDetail obj)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    var c = new List<CylinderMaster>();
                    var objCylinderlist = db.CustomerCylinderDetails.Where(a => a.cust_id == obj.cust_id).ToList();
                    foreach (var t in objCylinderlist)
                    {
                        CylinderMaster temp = db.CylinderMasters.Where(a => a.cylinderType == t.cylinderType).FirstOrDefault();
                        c.Add(temp);
                    }
                    obj.cylinders = c;
                    obj.users = repo.GetAllUsers();
                    obj.customers = repo.GetAllCustomers();
                    var tempTotal = Convert.ToDecimal(TempData["TotalAmount"].ToString());
                    if (tempTotal != obj.totalAmount)
                    {
                        var gst = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["gst"]);
                        var total = Convert.ToDecimal(obj.totalAmount) / gst;
                        var diff = Convert.ToDecimal(obj.totalAmount) - total;
                        obj.cgst = diff / 2;
                        obj.sgst = diff / 2;
                    }
                    repo.Update(obj);
                    ViewBag.SuccessMsg = "Record Updated Successfully";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";
            }
            var DeliveryDetaillist = repo.GetAllDeliveryDetails();

            return View("Listing", DeliveryDetaillist);
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

            var DeliveryDetaillist = repo.GetAllDeliveryDetails();
            return View("Listing", DeliveryDetaillist);
        }
        private string ToBase64ImageString(byte[] data)
        {
            return string.Format("data:image/jpeg;base64,{0}", Convert.ToBase64String(data));
        }
        public ActionResult PrintDeliveryDetail(int id)
        {
            try
            {
                NumberToEnglish num = new NumberToEnglish();
                var DeliveryDetails = repo.GetDeliveryDetailByID(id);
                var gst = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["gst"]);
                var total = Convert.ToDecimal(DeliveryDetails.totalAmount) / gst;
                var diff = Convert.ToDecimal(DeliveryDetails.totalAmount) - total;
                DeliveryDetails.cgst = diff / 2;
                DeliveryDetails.sgst = diff / 2;
                DeliveryDetails.balanceAmount = Convert.ToDecimal(DeliveryDetails.totalAmount);
                DeliveryDetails.totalAmount = total;
                DeliveryDetails.cylinderAmount = Convert.ToDecimal(DeliveryDetails.totalAmount / DeliveryDetails.filledCylinder);
                ViewBag.AmntInWord = num.changeCurrencyToWords(DeliveryDetails.balanceAmount.ToString());
                var taxamnt = DeliveryDetails.cgst + DeliveryDetails.sgst;
                ViewBag.TaxAmnt = string.Format("{0:0.00}", taxamnt);
                ViewBag.TaxAmntInWord = num.changeCurrencyToWords(ViewBag.TaxAmnt);

                if (DeliveryDetails.signature != null)
                    ViewBag.SignatureImage = ToBase64ImageString(DeliveryDetails.signature);
                return View(DeliveryDetails);
            }
            catch (Exception)
            {
                ViewBag.ErrorMsg = "Sorry Some Problem Occured";
            }
            return RedirectToAction("Listing");
        }

        [HttpPost]
        public JsonResult GetAmount(int val)
        {
            //var id = Convert.ToInt32(cust_id);
            var c = (from cont in db.CylinderMasters
                     join cont11 in db.CustomerCylinderDetails on cont.ID equals cont11.cylinder_Id
                     where cont11.cylinder_Id == val
                     select new
                     {
                         Discount = cont11.discount,
                         Amount = cont.amount
                     }).FirstOrDefault();
            var discount = Convert.ToDecimal(c.Discount);
            return Json(new { Success = "true", Data = new { amount = (c.Amount - discount) } });
        }

        //[HttpPost]
        //public JsonResult GetLoadCount(int val, DateTime date, int ctype)
        //{
        //    //Load Count Logic for delivery
        //    var c = (from cont in db.LoadCylinders
        //             join cont11 in db.LoadDetails on cont.ID equals cont11.load_Id
        //             where cont.user_id == val
        //             && cont.loadDate == date
        //             && cont11.cylinder_Id == ctype
        //             select new
        //             {
        //                 FilledCylinder = cont11.FilledCylinder,
        //             });

        //    var d = (from cont in db.DeliveryDetails
        //             where cont.userid == val
        //             && cont.C_deliveryDate == date
        //             && cont.cylinder_Id == ctype
        //             select new
        //             {
        //                 DelievryFilledCylinder = cont.filledCylinder,
        //                 DeliveryEmptyCylinder = cont.emptyCylinder,
        //                 DeliveryReplaceCylinder = cont.replacementCylinder
        //             });

        //    var FilledCylinderCount = 0;
        //    foreach (var i in c)
        //    {
        //        FilledCylinderCount = FilledCylinderCount + Convert.ToInt32(i.FilledCylinder);
        //    }

        //    var DeliveryFilledCylinderCount = 0;
        //    foreach (var i in d)
        //    {
        //        DeliveryFilledCylinderCount = DeliveryFilledCylinderCount + Convert.ToInt32(i.DelievryFilledCylinder);

        //    }

        //    //Unload Count Logic for delivery
        //    var c1 = (from cont in db.UnloadCylinders
        //              join cont11 in db.UnloadDetails on cont.ID equals cont11.unload_Id
        //              where cont.user_id == val
        //              && cont.UnloadDate == date
        //              && cont11.cylinder_Id == ctype
        //              && cont.DeliveryFlag != true
        //              select new
        //              {
        //                  EmptyCylinder = cont11.EmptyCylinder,
        //              });

        //    var d1 = (from cont in db.DeliveryDetails
        //              where cont.userid == val
        //              && cont.C_deliveryDate == date
        //              && cont.cylinder_Id == ctype
        //              && cont.unload_Flag == false
        //              select new
        //              {
        //                  DeliveryEmptyCylinder = cont.emptyCylinder,
        //              });

        //    var EmptyCylinderCount = 0;
        //    foreach (var i in c1)
        //    {
        //        EmptyCylinderCount = EmptyCylinderCount + Convert.ToInt32(i.EmptyCylinder);
        //    }

        //    var DeliveryEmptyCylinderCount = 0;
        //    foreach (var i in d1)
        //    {
        //        DeliveryEmptyCylinderCount = DeliveryEmptyCylinderCount + Convert.ToInt32(i.DeliveryEmptyCylinder);

        //    }

        //    // Replacement Logic
        //    var c2 = (from cont in db.UnloadCylinders
        //              join cont11 in db.UnloadDetails on cont.ID equals cont11.unload_Id
        //              where cont.user_id == val
        //              && cont.UnloadDate == date
        //              && cont11.cylinder_Id == ctype
        //              select new
        //              {
        //                  RejectedCylinder = cont11.RejectedCylinder,
        //              });

        //    var d2 = (from cont in db.DeliveryDetails
        //              where cont.userid == val
        //              && cont.C_deliveryDate == date
        //              && cont.cylinder_Id == ctype
        //              select new
        //              {
        //                  DeliveryRepalcementCylinder = cont.replacementCylinder,
        //              });

        //    var DeliveryRelpaceCylinderCount = 0;
        //    foreach (var i in d2)
        //    {
        //        DeliveryRelpaceCylinderCount = DeliveryRelpaceCylinderCount + Convert.ToInt32(i.DeliveryRepalcementCylinder);

        //    }

        //    var RejectedCylinderCount = 0;
        //    foreach (var i in c2)
        //    {
        //        RejectedCylinderCount = RejectedCylinderCount + Convert.ToInt32(i.RejectedCylinder);
        //    }


        //    return Json(new { Success = "true", Data = new { LoadfilledCylinder = FilledCylinderCount, DeliveryfilledCylinder = DeliveryFilledCylinderCount, UnloadEmptyCylinder = EmptyCylinderCount, DeliveryEmptyCylinder = DeliveryEmptyCylinderCount, UnloadRejectedCylinder = RejectedCylinderCount, DeliveryReplaceCylinder = DeliveryRelpaceCylinderCount } });
        //}

        [HttpPost]
        public ActionResult GetAmount_old(int? val)
        {
            if (val != null)
            {
                //CylinderMaster c = new CylinderMaster();
                // c = repo.GetCylinderType().Where(obj => obj.ID == val).FirstOrDefault();
                var c = (from cont in repo.GetCylinderType()
                         join cont11 in db.CustomerCylinderDetails on cont.ID equals cont11.cylinder_Id
                         where cont11.cylinder_Id == val
                         select new
                         {
                             Discount = cont11.discount,
                             Amount = cont.amount
                         }).FirstOrDefault();
                //   var t = db.CustomerCylinderDetails.Where(a => a.cylinderType == c.cylinderType).FirstOrDefault();
                var discount = Convert.ToDecimal(c.Discount);
                return Json(new { Success = "true", Data = new { amount = (c.Amount - discount) } });
            }
            else
                return Json(new { Success = "false" });

        }

        [HttpPost]
        public JsonResult GetCylinder(int cust_id)
        {
            //var id = Convert.ToInt32(cust_id);
            var objCylinderlist = db.CustomerCylinderDetails.Where(a => a.cust_id == cust_id).ToList();



            SelectList objCylinders = new SelectList(objCylinderlist, "cylinder_Id", "cylinderType", 0);

            return Json(objCylinders);
        }

        private string ConvertViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }

        public ActionResult SendEmail(int id)
        {
            try
            {
                NumberToEnglish num = new NumberToEnglish();
                var DeliveryDetails = repo.GetDeliveryDetailByID(id);
                var gst = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["gst"]);
                var total = Convert.ToDecimal(DeliveryDetails.totalAmount) / gst;
                var diff = Convert.ToDecimal(DeliveryDetails.totalAmount) - total;
                DeliveryDetails.cgst = diff / 2;
                DeliveryDetails.sgst = diff / 2;
                DeliveryDetails.balanceAmount = Convert.ToDecimal(DeliveryDetails.totalAmount);
                DeliveryDetails.totalAmount = total;
                DeliveryDetails.cylinderAmount = Convert.ToDecimal(DeliveryDetails.totalAmount / DeliveryDetails.filledCylinder);
                ViewBag.AmntInWord = num.changeCurrencyToWords(DeliveryDetails.balanceAmount.ToString());
                var taxamnt = DeliveryDetails.cgst + DeliveryDetails.sgst;
                ViewBag.TaxAmnt = string.Format("{0:0.00}", taxamnt);
                ViewBag.TaxAmntInWord = num.changeCurrencyToWords(ViewBag.TaxAmnt);

                var smtpServer = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["smtpServer"]);
                var smtpPort = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["smtpPort"]);
                var smtpUser = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["smtpUser"]);
                var smtpPwd = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["smtpPass"]);
                MailMessage message = new MailMessage();
                MailAddress Sender = new MailAddress(smtpUser);
                MailAddress receiver = new MailAddress(DeliveryDetails.CustomerDetail.email);
                SmtpClient smtp = new SmtpClient()
                {
                    Host = smtpServer,// "smtp.gmail.com", //for gmail
                    Port = Convert.ToInt32(smtpPort),//, //for gmail               
                    EnableSsl = false,
                    Credentials = new System.Net.NetworkCredential(smtpUser, smtpPwd)

                };
                message.From = Sender;
                message.Subject = "Invoice for delivery details- " + DeliveryDetails.voucherNo;
                message.To.Add(receiver);
                message.IsBodyHtml = true;
                message.Body = this.ConvertViewToString("~/Views/DeliveryDetail/PrintDeliveryDetail.cshtml", DeliveryDetails);
                smtp.Send(message);
                ViewBag.SuccessMsg = "Invoice sent Successfully";
            }
            catch (Exception)
            {
                ViewBag.Status = "Problem while sending email, Please check details.";

            }

            var DeliveryDetaillist = repo.GetAllDeliveryDetails();
            return View("Listing", DeliveryDetaillist);
        }


        /*Export to Excel*/
        [HttpPost]
        public FileResult Export()
        {
            if (Session["DeliveryDetaillist"] != null)
            {
                DataTable dt = new DataTable("Grid");
                dt.Columns.AddRange(new DataColumn[12] { new DataColumn("Delivery Date"),
                                               new DataColumn("Bill No"),
                                               new DataColumn("Kg"),
                                               new DataColumn("Name of Customer"),
                                               new DataColumn("Name of User"),
                                               new DataColumn("Amount"),
                                               new DataColumn("Balance"),
                                               new DataColumn("Filled Cylinder"),
                                               new DataColumn("Empty Cylinder"),
                                               new DataColumn("Discount"),
                                               new DataColumn("Sgst"),
                                               new DataColumn("Cgst")

               });

                var deliveryDetails = Session["DeliveryDetaillist"] as List<DeliveryDetail>;

                foreach (var deliveryDetail in deliveryDetails)
                {
                    dt.Rows.Add(deliveryDetail.C_deliveryDate, deliveryDetail.voucherNo, deliveryDetail.CylinderMaster.cylinderType,
                        deliveryDetail.CustomerDetail.companyName, deliveryDetail.tbluser.username, deliveryDetail.totalAmount, deliveryDetail.balanceAmount, deliveryDetail.filledCylinder,
                        deliveryDetail.emptyCylinder, deliveryDetail.CustomerDetail.discount, deliveryDetail.sgst, deliveryDetail.cgst);

                }

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(dt);
                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DeliveryDetailReport.xlsx");
                    }
                }
            }

            return null;

        }
    }
}

