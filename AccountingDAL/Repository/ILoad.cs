using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public interface ILoad
    {
        IEnumerable<LoadCylinder> GetAllLoadCylinders();
        LoadCylinder GetLoadCylinderByID(int id);
        IEnumerable<tbluser> GetAllUsers();
        IEnumerable<LoadDetail> GetLoadDetail(int id);
        IEnumerable<CylinderMaster> GetCylinderType();
        int Insert(LoadCylinder obj);
        void Update(LoadCylinder obj);
        void Delete(int id);
        void DeleteDetail(int id);


    }
}
