using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public interface IUnload
    {
        IEnumerable<UnloadCylinder> GetAllUnloadCylinders();
        UnloadCylinder GetUnloadCylinderByID(int id);
        IEnumerable<UnloadDetail> GetUnloadDetail(int id);
        IEnumerable<CylinderMaster> GetCylinderType();
        IEnumerable<tbluser> GetAllUsers();
        int Insert(UnloadCylinder obj);
        void Update(UnloadCylinder obj);
        void Delete(int id);
        void DeleteDetail(int id);


    }
}
