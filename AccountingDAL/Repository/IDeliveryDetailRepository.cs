using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public interface IDeliveryDetailRepository
    {
        IEnumerable<DeliveryDetail> GetAllDeliveryDetails();
        IEnumerable<tbluser> GetAllUsers();
        IEnumerable<CustomerDetail> GetAllCustomers();
        IEnumerable<CylinderMaster> GetCylinderType();
        DeliveryDetail GetDeliveryDetailByID(int id);
        int Insert(DeliveryDetail obj);
        void Update(DeliveryDetail obj);
        void Delete(int id);

    }
}
