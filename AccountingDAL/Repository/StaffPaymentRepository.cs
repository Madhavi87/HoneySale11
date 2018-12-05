using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public class StaffPaymentRepository : IStaffPayment
    {
        public honeysaleEntities context = null;

        public StaffPaymentRepository(honeysaleEntities context)
        {
            this.context = context;
        }

        public IEnumerable<tbluser> GetAllStaff()
        {
            return context.tblusers.ToList().Where(a => a.role_id == 2);
        }

        public IEnumerable<StaffPayment> GetAllStaffPaymentDetail()
        {
            return context.StaffPayments.ToList();
        }

        public StaffPayment GetStaffPaymentDetailByID(int id)
        {
            return context.StaffPayments.Where(obj => obj.ID == id).FirstOrDefault();
        }

        public int Insert(StaffPayment obj)
        {


            StaffPayment newStaffPaymentDetail = new StaffPayment();
            newStaffPaymentDetail.Staff__ID = obj.Staff__ID;
            newStaffPaymentDetail.Received_By = obj.Received_By;
            newStaffPaymentDetail.Received_On = obj.Received_On;
            newStaffPaymentDetail.Return_Amount = obj.Return_Amount;
            newStaffPaymentDetail.Return_Date = obj.Return_Date;
            newStaffPaymentDetail.Balance_Amount = obj.Balance_Amount;

            context.StaffPayments.Add(newStaffPaymentDetail);
            context.SaveChanges();

            return newStaffPaymentDetail.ID;
        }

        public void Update(StaffPayment obj)
        {
            StaffPayment newStaffPaymentDetail = new StaffPayment();
            newStaffPaymentDetail.Staff__ID = obj.Staff__ID;
            newStaffPaymentDetail.Received_By = obj.Received_By;
            newStaffPaymentDetail.Received_On = obj.Received_On;
            newStaffPaymentDetail.Return_Amount = obj.Return_Amount;
            newStaffPaymentDetail.Return_Date = obj.Return_Date;
            newStaffPaymentDetail.Balance_Amount = obj.Balance_Amount;
            context.StaffPayments.Add(newStaffPaymentDetail);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            StaffPayment newStaffPaymentDetail = context.StaffPayments.Where(obj => obj.Staff__ID == id).FirstOrDefault();
            context.StaffPayments.Where(obj => obj.Staff__ID == id).ToList().ForEach(obj => context.StaffPayments.Remove(obj));
            context.StaffPayments.Remove(newStaffPaymentDetail);
            context.SaveChanges();
        }

    }
}
