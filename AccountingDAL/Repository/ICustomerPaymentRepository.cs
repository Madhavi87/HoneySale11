using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public interface ICustomerPaymentRepository
    {
        IEnumerable<CustomerPaymentDetail> GetAllCustomerPaymentDetail();
        CustomerPaymentDetail GetCustomerPaymentDetailByID(int id); 
        int Insert(CustomerPaymentDetail obj);
        void Update(CustomerPaymentDetail obj);
        void Delete(int id);

    }
}
