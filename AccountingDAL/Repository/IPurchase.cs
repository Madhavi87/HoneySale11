using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public interface IPurchase
    {
        IEnumerable<PurchaseCylinder> GetAllPurchaseCylinder();
        PurchaseCylinder GetPurchaseByID(int id);
        IEnumerable<PurchaseDetail> GetPurchaseDetail(int id);
        IEnumerable<CylinderMaster> GetCylinderType();
        int Insert(PurchaseCylinder obj);
        void Update(PurchaseCylinder obj);
        void Delete(int id);
        void DeleteDetail(int id);

    }
}
