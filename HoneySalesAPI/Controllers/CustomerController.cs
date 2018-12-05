using HoneySaleDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace HoneySalesAPI.Controllers
{
    //[Authorize]


    public class CustomerController : ApiController
    {
        CustomerRepository repo = new CustomerRepository(new honeysaleEntities());
        CustomerPaymentRepository repopayment = new CustomerPaymentRepository(new honeysaleEntities());
        public System.Web.Mvc.JsonResult GetCustomerList()
        {
            // var session = HttpContext.Current.Session;
            //  string vusername = Convert.ToString(session["UserName"]);//= username;

            var objCustomerlist = repo.GetAllCustomers()
                                  .Select(c => new { Name = c.companyName, ID = c.cust_id })
                    .Distinct().OrderBy(ite => ite.Name).ToList();
            //var objCustomerlist = repo1.GetAllUsers()
            //               // .Where(c => c.username.ToUpper()
            //            //    .Contains(term.ToUpper()))
            //                .Select(c => new { Name = c.firstname, ID = c.ID })
            //                .Distinct().ToList();


            return new System.Web.Mvc.JsonResult()
            {
                Data = new
                {
                    result = objCustomerlist
                }
            };
        }

        public System.Web.Mvc.JsonResult GetCustomerList_withPaymentInfo()
        {
            var objCustomerlist = repo.GetAllCustomers()
                                  //s Where(ite => ite.cust_id == customerid)
                                  .Select(c => new { Name = c.companyName, ID = c.cust_id, Balance = c.openingBalance })
                    .Distinct().ToList();
            //var objCustomerlist = repo1.GetAllUsers()
            //               // .Where(c => c.username.ToUpper()
            //            //    .Contains(term.ToUpper()))
            //                .Select(c => new { Name = c.firstname, ID = c.ID })
            //                .Distinct().ToList();


            return new System.Web.Mvc.JsonResult()
            {
                Data = new
                {
                    result = objCustomerlist
                }
            };
        }



        public JsonResult SaveCustomerPaymentDetails(PaymentDetail iCustomerPaymentDetail)
        {

            string vErroMessage = "";
            bool status = false;
            try
            {
                honeysaleEntities db = new honeysaleEntities();
                string username = "";
                var userdetails = db.tblusers.Where(ite => ite.ID == iCustomerPaymentDetail.userid).FirstOrDefault();
                if (userdetails != null)
                    username = userdetails.username;

                CustomerPaymentDetail obj = new CustomerPaymentDetail();

                obj.cust_Id = iCustomerPaymentDetail.customerid;
                obj.balanceAmount = iCustomerPaymentDetail.balanceAmount;
                obj.PaidAmount = iCustomerPaymentDetail.paidAmount;
                obj.remark = iCustomerPaymentDetail.remark;
                //s obj.signature = signature;
                obj.date = DateTime.Now;
                obj.received = username;

                obj.signature = iCustomerPaymentDetail.signature;
                repopayment.Insert(obj);
                status = true;


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



        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyName"></param>
        /// <param name="address"></param>
        /// <param name="contact_Num_1"></param>
        /// <param name="email"></param>
        /// <param name="discount"></param>
        /// <returns></returns>
        /// //
        //http://adminapi.honeysales.in/API/Customer/SaveCustomerDetails?
        //companyName=ABC&address=Pune&contact_Num_1=9922798985&email=poojag@gmail.com&gstno=10
        public JsonResult SaveCustomerDetails(string companyName, string address,
          string contact_Num_1, string email, string gstno)
        {

            string vErroMessage = "";
            bool status = false;
            try
            {

                CustomerDetail custCylinder = new CustomerDetail();

                custCylinder.companyName = companyName;
                custCylinder.address = address;
                custCylinder.contact_Num_1 = contact_Num_1;
                custCylinder.email = email;
                custCylinder.gst = gstno;

                repo.Insert(custCylinder);



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


        public JsonResult GetCustomerInvoiceDetails(int customerid)
        {
            DeliveryDetailRepository repo = new DeliveryDetailRepository(new honeysaleEntities());
            //List<DeliveryDetail> delivertdetails = new List<DeliveryDetail>();
            //try
            // {
            var delivertdetails1 = repo.GetAllDeliveryDetails().
                                    Where(ite => ite.cust_id == customerid)
                                     .Select(c => new { InvoiceNo = c.voucherNo, PaidAmount = c.paidAmount, Balance = c.balanceAmount })
                                     .ToList();

            //  }
            // catch (Exception)
            //  {


            // }



            return new System.Web.Mvc.JsonResult()
            {
                Data = new
                {
                    result = delivertdetails1
                }
            };


        }

        public class PaymentDetail
        {


            public int customerid { get; set; }
            public string customername { get; set; }
            public decimal balanceAmount { get; set; }
            public string paymentoption { get; set; }
            public decimal paidAmount { get; set; }

            public string remark { get; set; }
            public byte[] signature { get; set; }
            public int userid { get; set; }
            public string username { get; set; }
        }

    }
}
