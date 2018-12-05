using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public interface IUserRepository
    {
        IEnumerable<tbluser> GetAllUsers();
        IEnumerable<tblrole> GetAllRole();
        tbluser GetUserByID(int id);
        int Insert(tbluser obj);
        void Update(tbluser obj);
        void Delete(int id);

    }
}
