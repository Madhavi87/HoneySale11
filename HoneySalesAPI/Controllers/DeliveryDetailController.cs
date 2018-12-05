using HoneySaleDAL;
using Num2Wrd;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace HoneySalesAPI.Controllers
{
    // [Authorize]
    public class DeliveryDetailsController : ApiController
    {
        DeliveryDetailRepository repo = new DeliveryDetailRepository(new honeysaleEntities());
        honeysaleEntities db = new honeysaleEntities();





        //// GET api/values
        //// [HttpPost]
        //public JsonResult SaveDeliveryDetails()
        //{
        //  //  var i =Request.Form[""];

        //    return new JsonResult()
        //    {
        //        Data = new
        //        {
        //            status = true,

        //        }
        //    };
        //}



        public JsonResult SaveDeliveryDetails(DeliveryDetailTemp iDeliveryDetail)
        {
            var data = new List<CylinderMaster>();
            string vErroMessage = "";
            bool status = false;



            try
            {
                if (iDeliveryDetail.UserId <= 0)
                {

                    vErroMessage = "Invalid User";
                    status = false;
                }
                else
                {
                    var result = (from cont in db.LoadCylinders
                                  join det in db.LoadDetails on cont.ID equals det.load_Id
                                  where cont.loadDate == iDeliveryDetail.C_deliveryDate && cont.user_id == iDeliveryDetail.UserId
                                  && det.cylinder_Id == iDeliveryDetail.cylinder_Id
                                  select det).ToList();

                    int loadcount = 0;
                    if (result != null)
                        loadcount = result.Sum(ite => ite.FilledCylinder).Value;


                    var deliverydetaillist = (from del in db.DeliveryDetails
                                              where del.C_deliveryDate == iDeliveryDetail.C_deliveryDate &&
                                              del.userid == iDeliveryDetail.UserId
                                              select del).ToList();

                    int deliverydetailcount = 0;
                    if (deliverydetaillist != null)
                        deliverydetailcount = deliverydetaillist.Sum(ite => ite.filledCylinder).Value + iDeliveryDetail.filledCylinder;
                    else
                        deliverydetailcount = iDeliveryDetail.filledCylinder;
                    //if (result != null && result.FilledCylinder > 0)
                    //{
                    //    status = true;
                    //}
                    if (result == null || deliverydetailcount > loadcount)
                    {
                        vErroMessage = "Cylinder Not available to User";
                        status = false;
                    }
                    else
                        status = true;
                    //else if (result.FilledCylinder <= 0)
                    //{
                    //    vErroMessage = "Cylinder Not Assigned to User";
                    //    status = false;
                    //}


                    // db.LoadCylinders.Where(ite => ite.loadDate == deliveryDate && ite.user_id == userid).ToList();
                }

                if (iDeliveryDetail.filledCylinder <= 0 && iDeliveryDetail.emptyCylinder <= 0 && iDeliveryDetail.replacementcylinder <= 0)
                {
                    status = false;
                    vErroMessage = "No Cylinder selected";

                }
                if (status)
                {
                    DeliveryDetail dd = new DeliveryDetail();

                    var lastvoucherNo = db.DeliveryDetails.Where(o => o.voucherNo != "LPG-E000").OrderByDescending(o => o.voucherNo).Select(m => m.voucherNo).FirstOrDefault();
                    if (lastvoucherNo == null)
                        lastvoucherNo = "0";
                    else
                        lastvoucherNo = lastvoucherNo.Replace("LPG-", "");

                    int last = Convert.ToInt32(lastvoucherNo);
                    var vVoucherNo = "";
                    if (iDeliveryDetail.filledCylinder > 0)
                        vVoucherNo = "LPG-" + (++last).ToString("D" + lastvoucherNo.Length);
                    else
                        vVoucherNo = "LPG-E000";


                    dd.voucherNo = vVoucherNo;
                    dd.C_deliveryDate = iDeliveryDetail.C_deliveryDate;
                    dd.cylinderAmount = iDeliveryDetail.cylinderAmount;
                    dd.filledCylinder = iDeliveryDetail.filledCylinder;
                    dd.emptyCylinder = iDeliveryDetail.emptyCylinder + iDeliveryDetail.replacementcylinder;
                    dd.totalAmount = iDeliveryDetail.totalAmount;
                    dd.paidAmount = iDeliveryDetail.paidAmount;
                    dd.balanceAmount = iDeliveryDetail.balanceAmount;
                    //  dd.customer = customername;
                    dd.cust_id = iDeliveryDetail.cust_id;
                    dd.cylinder_Id = iDeliveryDetail.cylinder_Id;
                    dd.signature = iDeliveryDetail.signature;
                    dd.userid = iDeliveryDetail.UserId;

                    var gst = 1.18;// Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["gst"]);
                    var total = Convert.ToDecimal(iDeliveryDetail.totalAmount) / Convert.ToDecimal(gst);
                    var diff = Convert.ToDecimal(iDeliveryDetail.totalAmount) - total;
                    dd.cgst = diff / 2;
                    dd.sgst = diff / 2;


                    int id = repo.Insert(dd);
                    vErroMessage = "Record Added Successfully";
                    status = true;
                    if (id > 0)
                    {
                        try
                        {
                            SendEmail(id);
                        }
                        catch (Exception)
                        {

                        }
                    }
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


        public JsonResult GetCylinderAmount(int? val)
        {
            var status = false;
            decimal amount = 0;
            if (val != null)
            {
                try
                {
                    CylinderMaster c = new CylinderMaster();
                    c = repo.GetCylinderType().Where(obj => obj.ID == val).FirstOrDefault();
                    status = true;
                    amount = c.amount.Value;
                }
                catch (Exception)
                {

                    status = false; ;
                    amount = 0;
                }
            }


            return new JsonResult()
            {
                Data = new
                {
                    status = status,
                    amount = amount
                }
            };

        }

        public JsonResult GetCylinderAmountByCustomer(int cylinderid, int customerId)
        {
            var status = false;
            decimal amount = 0;
            if (cylinderid > 0)
            {
                try
                {
                    CylinderMaster c = new CylinderMaster();
                    c = repo.GetCylinderType().Where(obj => obj.ID == cylinderid).FirstOrDefault();
                    var customerdetail = db.CustomerCylinderDetails.Where(ite => ite.cylinder_Id == cylinderid && ite.cust_id == customerId).FirstOrDefault();

                    decimal discount = customerdetail == null ? 0 : Convert.ToDecimal(customerdetail.discount);
                    status = true;
                    amount = c.amount.Value - discount;
                }
                catch (Exception)
                {

                    status = false; ;
                    amount = amount;
                }
            }


            return new JsonResult()
            {
                Data = new
                {
                    status = status,
                    amount = amount
                }
            };

        }

        public JsonResult GetCylinderMasterListbyCustomer(int customerid)
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

            var cylinderlist = db.CustomerCylinderDetails.
                Where(ite => ite.cust_id == customerid)
                               .Select(c => new { cylinderType = c.CylinderMaster.cylinderType, ID = c.CylinderMaster.ID })
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

        public void SendEmail(int id)
        {
            try
            {

                NumberToEnglish num = new NumberToEnglish();
                var DeliveryDetails = repo.GetDeliveryDetailByID(id);
                var cgst = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["Cgst"]) / 100;
                var sgst = Convert.ToDecimal(System.Configuration.ConfigurationManager.AppSettings["Sgst"]) / 100;
                var total = Convert.ToDecimal(DeliveryDetails.totalAmount);
                DeliveryDetails.cgst = total * cgst;
                DeliveryDetails.sgst = total * sgst;
                DeliveryDetails.balanceAmount = total + DeliveryDetails.cgst + DeliveryDetails.sgst;
                var AmntInWord = num.changeNumericToWords(DeliveryDetails.balanceAmount.ToString());
                var TaxAmnt = DeliveryDetails.cgst + DeliveryDetails.sgst;
                var TaxAmntInWord = num.changeNumericToWords((DeliveryDetails.cgst + DeliveryDetails.sgst).ToString());

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
                    Port = Convert.ToInt32(smtpPort),//587, //for gmail               
                    EnableSsl = false,
                    Credentials = new System.Net.NetworkCredential(smtpUser, smtpPwd)

                };
                message.From = Sender;
                message.Subject = "Invoice for delivery details- " + DeliveryDetails.voucherNo;
                message.To.Add(receiver);
                message.IsBodyHtml = true;
                message.Body = RenderViewToString("DeliveryDetai", "~/Views/DeliveryDetail/PrintDeliveryDetail.cshtml", DeliveryDetails); //this.ConvertViewToString(DeliveryDetails, AmntInWord.ToString(), TaxAmnt.ToString(), TaxAmntInWord.ToString());
                smtp.Send(message);

            }
            catch (Exception ex)
            {


            }
        }

        private string ConvertViewToString(DeliveryDetail model, string AmntInWord, string TaxAmnt, string TaxAmntInWord)
        {
            return "<html lang = 'en'>" +
   "<head>" +
     "<meta charset = 'UTF-8' >" +
      "<title > Sample Invoice </ title >" +
       "<link rel = 'stylesheet' href = '~/Content/css/bootstrap.min.css' >" +
           "  </ head >" +
  "<body >" +
    "<div class='container'>" +
     "<div class='row'>" +
         "<span style = 'text-align:center' > <h3 > Tax Invoice</h3></span>" +
    "<table class='table table-bordered tblfont' border='1'>" +
    "<thead>" +
      "<tr>" +
            "<td colspan = '2' rowspan='3'><h5><b>HONEY SALES CORPORATION</b></h5>" +
                "<p> 7,ASHOK VIHAR, 2nd floor,<br />" +
                    "XLO POINT, MIDC AMBAD,<br />" +
                    "AMBAD-422010<br />" +
                    "GSTIN/UIN: 27AABPT1774F1ZM<br />" +
                    "State Name: Maharashtra, Code :27" +
                    "E-Mail: honeysalescorp @gmail.com<br />" +
                "</p> " +
             "</td>" +
              "<td colspan = '3' > Invoice No.<br />" + model.voucherNo + "</td>" +
              "<td colspan = '2' > Dated <br />" + model.C_deliveryDate + "</ td >" +
         "</ tr >" +
        "<tr >" +
           " <td colspan= '3' > Delivery Note<br /></td>" +
           " <td colspan = '2' > Mode / Terms of Payment<br /></td>" +
        "</tr>" +
         "<tr>" +
            "<td colspan = '3' > Supplier's Ref<br /></td>" +
            "<td colspan = '2' > Other Reference(s)<br /></td>" +
       "</tr>" +
       "<tr>" +
             "<td colspan = '2' rowspan= '4' >" +
                  "Buyer" +
                "<h5> <b >" + model.CustomerDetail.companyName + "</ b ></ h5 >" +
               "<p >" + model.CustomerDetail.address + "</ p >" +
            "</ td >" +
             "<td colspan= '3' > Buyer's Order No.<br />" +
            "</td>" +
              "<td colspan = '2' > Dated <br />" +
             "</td>" +
      "</ tr >" +
        "<tr >" +
             "<td colspan= '3' > Dispatch Document No.<br />" +
             "</td>" +
              "<td colspan = '2' > Delivery Note Date<br />" +
             "</td>" +
        "</tr>" +
        "<tr>" +
            "<td colspan = '3' > Dispatch through<br />" +
            "</td>" +
              "<td colspan = '2' > Destination <br />" +
             "</ td >" +
        "</ tr >" +
        "<tr >" +
             "<td colspan= '5' > Terms of delivery<br />" +
             "</td>              " +
        "</tr>      " +
        "<tr>" +
           "<td>SI No.</td>" +
            "<td >Description of Good</td>" +
            "<td>HSN/SAC</td>" +
            "<td>Quantity</td>" +
            "<td>Rate</td>" +
            "<td>per</td>" +
            "<td>Amount</td>" +
        "</tr>    " +
    "</thead>" +
    "<tbody>" +
         "<tr>" +
            "<td>1</td>" +
            "<td><b>LPG CYLINDER" + model.CylinderMaster.cylinderType + "</b></td>" +
            "<td>2711</td>" +
            "<td><b>" + model.filledCylinder + "nos</b></td>" +
            "<td>" + string.Format("{0:0.00}", model.cylinderAmount) + "</td>" +
            "<td>nos</td>" +
            "<td>" + string.Format("{0:0.00}", model.totalAmount) + "</td>" +
        "</tr>" +
        "<tr><td colspan = '7' ><br /><br /></ td ></ tr >" +
         "<tr >" +
        "<td ></ td >" +
        "<td > Total </ td >" +
        "<td ></ td >" +
        "<td ><b >" + model.filledCylinder + "nos </b></td>" +
        "<td></td>" +
        "<td></td>" +
        "<td>&#8377; <b>" + string.Format("{0:0.00}", model.balanceAmount) + "</b></td>" +
      "</tr>" +
      "<tr>" +
          "<td colspan = '7' ><span > Amount Chargeble(in words) <br /> <b>" + AmntInWord + "Only</b></span>" +
            "<span style = 'float:right' > E & O.E <br /></ span >" +

                "</ td >      " +
            "</ tr >      " +
            "<tr >      " +
                "<td rowspan= '2' > HSN / SAC </ td >      " +
                "<td rowspan= '2' > Taxable <br /> Value </ td >" +
                "<td colspan= '2' > Central Tax</td>" +
                "<td colspan = '2' > State Tax</td>" +
                "<td rowspan = '2' > Total <br /> Tax Amount</td>" +
            "</tr>" +
        "<tr>          " +
          "<td>Rate</td>" +
                "<td>Amount</td>" +
                "<td>Rate</td>" +
                "<td>Amount</td>" +
            "</tr>" +
        "<tr>" +
          "<td>2711</td>" +
          "<td>" + string.Format("{0:0.00}", model.totalAmount) + "</td> " +
          "<td>" + System.Configuration.ConfigurationManager.AppSettings["Cgst"].ToString() + "%</td> " +
          "<td>" + string.Format("{0:0.00}", model.cgst) + "</td> " +
          "<td>" + System.Configuration.ConfigurationManager.AppSettings["Sgst"].ToString() + "%</td> " +
          "<td>" + string.Format("{0:0.00}", model.sgst) + "</td> " +

          "<td>" + string.Format("{0:0.00}", TaxAmnt) + "</td> " +
      "</tr>" +
         "<tr>" +
          "<td><b>Total</b></td>" +
          "<td><b>" + string.Format("{0:0.00}", model.totalAmount) + "</b></td> " +
          "<td></td>" +
          "<td><b>" + string.Format("{0:0.00}", model.cgst) + "</b></td>" +
          "<td></td>" +
          "<td><b>" + string.Format("{0:0.00}", model.sgst) + "</b></td>" +
          "<td><b>" + string.Format("{0:0.00}", TaxAmnt) + "</b></td>" +
      "</tr>" +
        " <tr>" +
          "<td colspan = '7' style='border-bottom:none'>Amount Chargeble(in words): <b>" + TaxAmntInWord + "Only</b></td>" +
      "</tr>" +
    "</tbody>" +
        "<tfoot>" +
           " <tr>" +
            "<td colspan = '3' style= 'border-top:none' ><b > Company's PAN : AABPT1774F</b>" +
                  " <p>Declaration: " +
                      "We declare thatthis invoice shows the<br />" +
                     "actual price of the goods described<br />" +
                      "and that all particulars are true and correct." +
                "</p>" +
            "</td>" +
            "<td colspan = '4' style= 'text-align:right' ><h5 > For Honey Sales Corporation</h5>" +
                  "<img src = '~/Content/img/CustSign.png' width= '50'  height= '50' /><br />" +
                  "Customer Sign<br />" +
                 "<img src = '~/Content/img/OwnerSign.png' width= '50'  height= '50' /><br />" +
                  "Authorised Signatory" +
              "</td>" +
              "</tr>" +
            "<tr>" +
            "<td colspan = '7' ><div >  " +
             " <button id= 'printbutton' onclick= 'PrintPage()' > Print </ button >" +

              "</ div ></ td >  " +
             " </ tr >  " +
         " </ tfoot >  " +
         "</ table >  " +
          "</ div >  " +
      "</ div >  " +
    "</ body >" +
  "</ html > ";
        }

        public static string RenderViewToString(string controllerName, string viewName, object viewData)
        {
            var context = System.Web.HttpContext.Current;
            var contextBase = new HttpContextWrapper(context);
            var routeData = new RouteData();
            routeData.Values.Add("controller", controllerName);

            var controllerContext = new ControllerContext(contextBase,
                                                         routeData,
                                                         new EmptyController());

            var razorViewEngine = new RazorViewEngine();
            var razorViewResult = razorViewEngine.FindView(controllerContext,
                                                           viewName,
                                                           "",
                                                           false);

            var writer = new StringWriter();
            var viewContext = new ViewContext(controllerContext,
                                              razorViewResult.View,
                                            new ViewDataDictionary(viewData),
                                              new TempDataDictionary(),
                                              writer);
            razorViewResult.View.Render(viewContext, writer);

            return writer.ToString();
        }

        class EmptyController : ControllerBase
        {
            protected override void ExecuteCore() { }
        }



    }
    public class DeliveryDetailTemp
    {

        ///s  [Required(ErrorMessage = "please select date")]
       // [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public System.DateTime C_deliveryDate { get; set; }
        public int UserId { get; set; }
        public int replacementcylinder { get; set; }
        public string voucherNo { get; set; }
        public decimal cylinderAmount { get; set; }
        public int filledCylinder { get; set; }
        public int emptyCylinder { get; set; }
        public decimal totalAmount { get; set; }
        public decimal paidAmount { get; set; }
        public decimal balanceAmount { get; set; }
        //  [Required(ErrorMessage = "please select customer")]
        //public string customer { get; set; }
        public int cust_id { get; set; }
        //  [Required(ErrorMessage = "please select cylinder")]
        public int cylinder_Id { get; set; }
        public int ID { get; set; }
        public byte[] signature { get; set; }

    }
}