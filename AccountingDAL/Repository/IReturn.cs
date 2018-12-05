using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public interface IReturn
    {
        IEnumerable<ReturnCylinder> GetAllReturns();
        ReturnCylinder GetReturnByID(int id);
        IEnumerable<ReturnDetail> GetReturnDetail(int id);
        IEnumerable<CylinderMaster> GetCylinderType();
        int Insert(ReturnCylinder obj);
        void Update(ReturnCylinder obj);
        void Delete(int id);
        void DeleteDetail(int id);


    }
}
