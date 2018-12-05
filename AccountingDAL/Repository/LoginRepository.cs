using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public class LoginRepository
    {
        public honeysaleEntities context = null;

        public LoginRepository()
        {
            context = new honeysaleEntities();
        }


        public tbluser Login(string username, string password)
        {

            var userlist = context.tblusers.ToList();
            var output = userlist.Where(obj => (obj.username == username) && (obj.passwordHash == password)
            && obj.isActive == true).FirstOrDefault();//&&(obj.role_id.Value == 2)


            return output;
        }
        public IEnumerable<CustomerPaymentDetail> GetAllCustomerPaymentDetail()
        {
            return context.CustomerPaymentDetails.ToList();
        }

        public CustomerPaymentDetail GetCustomerPaymentDetailByID(int id)
        {
            return context.CustomerPaymentDetails.Where(obj => obj.cust_Id == id).FirstOrDefault();
        }

        public int Insert(CustomerPaymentDetail obj)
        {


            CustomerPaymentDetail newCustomerPaymentDetail = new CustomerPaymentDetail();
            newCustomerPaymentDetail.cust_Id = obj.cust_Id;
            newCustomerPaymentDetail.balanceAmount = obj.balanceAmount;
            newCustomerPaymentDetail.PaidAmount = obj.PaidAmount;
            newCustomerPaymentDetail.date = obj.date;
            newCustomerPaymentDetail.received = obj.received;

            newCustomerPaymentDetail.remark = obj.remark;
            newCustomerPaymentDetail.signature = obj.signature;
            context.CustomerPaymentDetails.Add(newCustomerPaymentDetail);
            context.SaveChanges();

            CustomerDetail objCustomerDetails = context.CustomerDetails.Where(ite => ite.cust_id == obj.cust_Id).FirstOrDefault();
            if (objCustomerDetails != null)
            {
                objCustomerDetails.openingBalance = objCustomerDetails.openingBalance - obj.PaidAmount;
                context.SaveChanges();
            }
            return newCustomerPaymentDetail.cust_Id;
        }

        public void Update(CustomerPaymentDetail obj)
        {
            CustomerPaymentDetail newCustomerPaymentDetail = new CustomerPaymentDetail();
            newCustomerPaymentDetail.cust_Id = obj.cust_Id;
            newCustomerPaymentDetail.balanceAmount = obj.balanceAmount;
            newCustomerPaymentDetail.PaidAmount = obj.PaidAmount;
            newCustomerPaymentDetail.date = obj.date;
            newCustomerPaymentDetail.received = obj.received;
            newCustomerPaymentDetail.remark = obj.remark;
            newCustomerPaymentDetail.signature = obj.signature;
            context.CustomerPaymentDetails.Add(newCustomerPaymentDetail);
            context.SaveChanges();
        }

        public void Delete(int id)
        {
            CustomerPaymentDetail newCustomerPaymentDetail = context.CustomerPaymentDetails.Where(obj => obj.cust_Id == id).FirstOrDefault();
            context.CustomerPaymentDetails.Where(obj => obj.cust_Id == id).ToList().ForEach(obj => context.CustomerPaymentDetails.Remove(obj));
            context.CustomerPaymentDetails.Remove(newCustomerPaymentDetail);
            context.SaveChanges();
        }

    }
}
