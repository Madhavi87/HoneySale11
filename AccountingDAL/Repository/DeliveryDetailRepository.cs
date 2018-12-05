using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public class DeliveryDetailRepository : IDeliveryDetailRepository
    {
        public honeysaleEntities context = null;

        public DeliveryDetailRepository(honeysaleEntities context)
        {
            this.context = context;

        }

        public IEnumerable<DeliveryDetail> GetAllDeliveryDetails()
        {
            return context.DeliveryDetails.ToList();
        }

        public IEnumerable<CustomerDetail> GetAllCustomers()
        {
            return context.CustomerDetails.ToList();
        }

        public IEnumerable<tbluser> GetAllUsers()
        {
            return context.tblusers.ToList();
        }

        public DeliveryDetail GetDeliveryDetailByID(int id)
        {
            return context.DeliveryDetails.Where(obj => obj.ID == id).FirstOrDefault();
        }
        public IEnumerable<CylinderMaster> GetCylinderType()
        {
            return context.CylinderMasters.ToList();
        }

        public int Insert(DeliveryDetail obj)
        {
            try
            {
                DeliveryDetail newdeliverydetail = new DeliveryDetail();
                newdeliverydetail.C_deliveryDate = obj.C_deliveryDate;
                newdeliverydetail.voucherNo = obj.voucherNo;
                newdeliverydetail.cylinder_Id = obj.cylinder_Id;
                newdeliverydetail.cust_id = obj.cust_id;
                newdeliverydetail.cylinderAmount = obj.cylinderAmount;
                newdeliverydetail.emptyCylinder = obj.emptyCylinder;
                newdeliverydetail.filledCylinder = obj.filledCylinder;
                newdeliverydetail.replacementCylinder = obj.replacementCylinder;
                newdeliverydetail.paidAmount = obj.paidAmount;
                newdeliverydetail.totalAmount = obj.totalAmount;
                newdeliverydetail.sgst = Math.Round(obj.sgst.Value);
                newdeliverydetail.cgst = Math.Round(obj.cgst.Value);
                newdeliverydetail.balanceAmount = obj.balanceAmount;
                newdeliverydetail.userid = obj.userid;
                if (obj.signature != null)
                    newdeliverydetail.signature = obj.signature;
                if (obj.phone != null)
                {
                    newdeliverydetail.phone = obj.phone;
                }

                if (obj.filledCylinder <= 0)
                    obj.voucherNo = "LPG-E000";
                context.DeliveryDetails.Add(newdeliverydetail);
                context.SaveChanges();

                CustomerDetail objCustomerDetails = context.CustomerDetails.Where(ite => ite.cust_id == obj.cust_id).FirstOrDefault();
                if (objCustomerDetails != null)
                {
                    objCustomerDetails.openingBalance = objCustomerDetails.openingBalance + obj.balanceAmount;
                    context.SaveChanges();
                }

                CustomerCylinderDetail objCustomerCylinderDetail = context.CustomerCylinderDetails.Where(ite => ite.cust_id == obj.cust_id && ite.cylinder_Id == obj.cylinder_Id).FirstOrDefault();
                if (objCustomerCylinderDetail != null)
                {
                    if (obj.filledCylinder > 0)
                        objCustomerCylinderDetail.totalCylinder = objCustomerCylinderDetail.totalCylinder + obj.filledCylinder;
                    if (obj.emptyCylinder > 0)
                        objCustomerCylinderDetail.totalCylinder = objCustomerCylinderDetail.totalCylinder - obj.emptyCylinder;
                    if (obj.replacementCylinder > 0)
                        objCustomerCylinderDetail.totalCylinder = objCustomerCylinderDetail.totalCylinder - obj.replacementCylinder;





                    context.SaveChanges();
                }
                return newdeliverydetail.ID;
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                // ViewBag.ErrorMsg = "Sorry Some Problem Occured";
                //dd.cylinders = repo.GetCylinderType();
                //return View(dd);
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity.ToString(),
                            validationError.ErrorMessage);
                        // raise a new exception nesting
                        // the current instance as InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;

            }
        }

        public void Update(DeliveryDetail obj)
        {
            DeliveryDetail newdeliverydetail = context.DeliveryDetails.Where(obj1 => obj1.ID == obj.ID).FirstOrDefault();
            newdeliverydetail.C_deliveryDate = obj.C_deliveryDate;
            newdeliverydetail.cylinder_Id = obj.cylinder_Id;
            newdeliverydetail.cylinderAmount = obj.cylinderAmount;
            newdeliverydetail.emptyCylinder = obj.emptyCylinder;
            newdeliverydetail.filledCylinder = obj.filledCylinder;
            newdeliverydetail.replacementCylinder = obj.replacementCylinder;
            newdeliverydetail.paidAmount = obj.paidAmount;
            newdeliverydetail.totalAmount = obj.totalAmount;
            newdeliverydetail.balanceAmount = obj.balanceAmount;
            newdeliverydetail.sgst = obj.sgst;
            newdeliverydetail.cgst = obj.cgst;
            newdeliverydetail.cust_id = obj.cust_id;
            //  newdeliverydetail.CustomerDetail.companyName = obj.CustomerDetail.companyName;
            newdeliverydetail.userid = obj.userid;
            newdeliverydetail.phone = obj.phone;

            context.SaveChanges();
        }

        public void Delete(int id)
        {
            DeliveryDetail newdeliverydetail = context.DeliveryDetails.Where(obj => obj.ID == id).FirstOrDefault();
            context.DeliveryDetails.Remove(newdeliverydetail);
            context.SaveChanges();
        }
    }
}
