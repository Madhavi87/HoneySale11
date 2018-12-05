using HoneySaleDAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;

namespace HoneySalesAPI.Controllers
{
    public class LoginController : ApiController
    {

        honeysaleEntities _db = new honeysaleEntities();


        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values

        public JsonResult Login(string username, string password)
        {
            string vErroMessage = "";
            bool success = true;
            string role = "Staff";
            int userid = 0;
            // username = uname;
            try
            {

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    vErroMessage = "Please fill mandatory fields";
                    success = false;
                }
                //if (!success)
                //    return vErroMessage;
                bool isActive = true;
                //System.Data.Objects.ObjectParameter status = new System.Data.Objects.ObjectParameter("status", typeof(bool));
                // System.Data.Objects.ObjectParameter msg = new System.Data.Objects.ObjectParameter("message", typeof(string));
                password = Encrypt(password);
                var userlist = _db.tblusers.ToList();
                var output = userlist.Where(obj => (obj.username == username) && (obj.passwordHash == password)
                && obj.isActive == isActive).FirstOrDefault();//&&(obj.role_id.Value == 2)

                if (output != null)
                {
                    if (output.role_id == 3 || output.role_id == 1)
                        role = "Manager";
                    //SessionStateAttribute["UserId"] = username;
                    FormsAuthentication.SetAuthCookie(username, true);
                    // Session["UserName"] = UR.UserName.ToUpper();

                    userid = output.ID;
                    // var session = HttpContext.Current.Session;
                    // session["UserName"] = username;
                    vErroMessage = "Success";
                    success = true;
                }

                else
                {
                    vErroMessage = "Invalid Credentials!";
                    success = false;
                }
            }
            catch (Exception ex)
            {
                success = false;
                vErroMessage = ex.Message + "Error Occured while your transaction!";
            }
            return new JsonResult()
            {
                Data = new
                {
                    Status = success,
                    Message = vErroMessage,
                    role = role,
                    userid = userid
                }
            };


        }


        public string Encrypt(string clearText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    clearText = Convert.ToBase64String(ms.ToArray());
                }
            }
            return clearText;
        }

        public string Decrypt(string cipherText)
        {
            string EncryptionKey = "MAKV2SPBNI99212";
            byte[] cipherBytes = Convert.FromBase64String(cipherText);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    cipherText = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return cipherText;
        }


    }


}

