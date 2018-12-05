using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public class CustomerRepository : ICustomerRepository
    {
        public honeysaleEntities context = null;

        public CustomerRepository(honeysaleEntities context)
        {
            this.context = context;
        }

        public IEnumerable<CustomerDetail> GetAllCustomers()
        {
            return context.CustomerDetails.ToList();
        }
        public IEnumerable<CylinderMaster> GetCylinderType()
        {
            return context.CylinderMasters.ToList();
        }
        public IEnumerable<CustomerCylinderDetail> GetCustomerCylinderDetail(int id)
        {
            return context.CustomerCylinderDetails.Where(obj => obj.cust_id == id).ToList();
        }
        public CustomerDetail GetCustomerByID(int id)
        {
            return context.CustomerDetails.Where(obj => obj.cust_id == id).FirstOrDefault();
        }

        public CustomerDetail GetCustomerByName(string str)
        {
           return context.CustomerDetails.Where(obj => obj.companyName == str).FirstOrDefault();
        }

        public int Insert(CustomerDetail obj)
        {

            CustomerDetail newCustomerMaster = new CustomerDetail();
         //   CustomerCylinderDetail newCustomerCylinderDetail = new CustomerCylinderDetail();
            newCustomerMaster.companyName = obj.companyName;
            newCustomerMaster.address = obj.address;
            newCustomerMaster.contact_Num_1 = obj.contact_Num_1;
            newCustomerMaster.contact_Num_2 = obj.contact_Num_2;
            newCustomerMaster.email = obj.email;
            newCustomerMaster.discount = obj.discount;
            newCustomerMaster.gst = obj.gst;
            newCustomerMaster.balanceAmount = obj.balanceAmount;
            newCustomerMaster.openingBalance = obj.openingBalance;
            context.CustomerDetails.Add(newCustomerMaster);
            context.SaveChanges();
            if (obj.CustomerCylinderDetail != null)
            {
                foreach (var i in obj.CustomerCylinderDetail)
                {
                    CustomerCylinderDetail newCustomerCylinderDetail = new CustomerCylinderDetail();
                    newCustomerCylinderDetail.cylinder_Id = i.cylinder_Id;
                    newCustomerCylinderDetail.cylinderType = i.cylinderType;
                    newCustomerCylinderDetail.discount = i.discount;
                    newCustomerCylinderDetail.totalCylinder = i.totalCylinder;
                    newCustomerCylinderDetail.openingCylinder = i.openingCylinder;
                    newCustomerCylinderDetail.cust_id = newCustomerMaster.cust_id;
                    context.CustomerCylinderDetails.Add(newCustomerCylinderDetail);
                    context.SaveChanges();
                }
            }
           
            return newCustomerMaster.cust_id;
        }

        public void Update(CustomerDetail obj)
        {
            CustomerDetail newCustomerMaster = context.CustomerDetails.Where(obj1 => obj1.cust_id == obj.cust_id).FirstOrDefault();
            CustomerCylinderDetail newCustomerCylinderDetail = new CustomerCylinderDetail();

            newCustomerMaster.companyName = obj.companyName;
            newCustomerMaster.address = obj.address;
            newCustomerMaster.contact_Num_1 = obj.contact_Num_1;
            newCustomerMaster.contact_Num_2 = obj.contact_Num_2;
            newCustomerMaster.email = obj.email;
            newCustomerMaster.discount = obj.discount;
            newCustomerMaster.gst = obj.gst;
            newCustomerMaster.openingBalance = obj.openingBalance;
            newCustomerMaster.balanceAmount = obj.balanceAmount;
            context.SaveChanges();
            if (obj.CustomerCylinderDetailForEdit != null)
            {
                foreach (var i in obj.CustomerCylinderDetailForEdit)
                {
                    newCustomerCylinderDetail.cylinder_Id = i.cylinder_Id;
                    newCustomerCylinderDetail.cylinderType = i.cylinderType;
                    newCustomerCylinderDetail.discount = i.discount;
                    newCustomerCylinderDetail.totalCylinder = i.totalCylinder;
                    newCustomerCylinderDetail.openingCylinder = i.openingCylinder;
                    newCustomerCylinderDetail.cust_id = newCustomerMaster.cust_id;
                    context.CustomerCylinderDetails.Add(newCustomerCylinderDetail);
                    context.SaveChanges();
                }
            }
        }

        public void Delete(int id)
        {
            CustomerDetail newCustomerMaster = context.CustomerDetails.Where(obj => obj.cust_id == id).FirstOrDefault();
            context.CustomerCylinderDetails.Where(obj => obj.cust_id == id).ToList().ForEach(obj => context.CustomerCylinderDetails.Remove(obj));
            context.CustomerDetails.Remove(newCustomerMaster);
            context.SaveChanges();
        }
        public void DeleteDetail(int id)
        {
            CustomerCylinderDetail newCustomerCylinder = context.CustomerCylinderDetails.Where(obj => obj.cust_id == id).FirstOrDefault();
            context.CustomerCylinderDetails.Remove(newCustomerCylinder);
            context.SaveChanges();
        }
    }
}
