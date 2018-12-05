using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public interface ICylinderRepository
    {
        IEnumerable<CylinderMaster> GetAllCylinders();
        CylinderMaster GetCylinderByID(int id);
        int Insert(CylinderMaster obj);
        void Update(CylinderMaster obj);
        void Delete(int id);

    }
}
