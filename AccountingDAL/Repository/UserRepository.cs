using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public class UserRepository : IUserRepository
    {
        public honeysaleEntities contextu = null;

        public UserRepository(honeysaleEntities contextu)
        {
            this.contextu = contextu;
        }

        public IEnumerable<tbluser> GetAllUsers()
        {
            
            return contextu.tblusers.ToList();
        }
        public IEnumerable<tblrole> GetAllRole()
        {
            return contextu.tblroles.ToList();
        }
        public tbluser GetUserByID(int id)
        {
            return contextu.tblusers.Where(obj => obj.ID == id).FirstOrDefault();
            
        }

        public int Insert(tbluser obj)
        {

            tbluser newUserMaster = new tbluser();
            newUserMaster.firstname = obj.firstname;
            newUserMaster.lastname = obj.lastname;
            newUserMaster.passwordHash = obj.passwordHash;
            newUserMaster.username = obj.username;
            newUserMaster.role_id = obj.role_id;
            newUserMaster.phone = obj.phone;
            newUserMaster.isActive = obj.isActive;

            contextu.tblusers.Add(newUserMaster);
            contextu.SaveChanges();
            return newUserMaster.ID;
        }

        public void Update(tbluser obj)
        {
            tbluser newUserMaster = contextu.tblusers.Where(obj1 => obj1.ID == obj.ID).FirstOrDefault();
            newUserMaster.firstname = obj.firstname;
            newUserMaster.lastname = obj.lastname;
            newUserMaster.passwordHash = obj.passwordHash;
            newUserMaster.username = obj.username;
            newUserMaster.role_id = obj.role_id;
            newUserMaster.phone = obj.phone;
            newUserMaster.isActive = obj.isActive;

            contextu.SaveChanges();
        }

        public void Delete(int id)
        {
            tbluser newUserMaster = contextu.tblusers.Where(obj => obj.ID == id).FirstOrDefault();
            contextu.tblusers.Remove(newUserMaster);
            contextu.SaveChanges();
        }
    }
}
