using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public interface IStaffPayment
    {
        IEnumerable<StaffPayment> GetAllStaffPaymentDetail();
        IEnumerable<tbluser> GetAllStaff();
        StaffPayment GetStaffPaymentDetailByID(int id); 
        int Insert(StaffPayment obj);
        void Update(StaffPayment obj);
        void Delete(int id);

    }
}
