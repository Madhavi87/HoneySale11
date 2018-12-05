using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public interface ICustomerRepository
    {
        IEnumerable<CustomerDetail> GetAllCustomers();
        CustomerDetail GetCustomerByID(int id);
        CustomerDetail GetCustomerByName(string str);
        IEnumerable<CylinderMaster> GetCylinderType();
        IEnumerable<CustomerCylinderDetail> GetCustomerCylinderDetail(int id);
        int Insert(CustomerDetail obj);
        void Update(CustomerDetail obj);
        void Delete(int id);

    }
}
