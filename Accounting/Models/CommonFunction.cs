using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace HoneySale.Models
{
    public class CommonFunction
    {

        public static string GetShortDayFormat(object idate)
        {
            if (idate != null)
            {
                return String.Format("{0:d MMM yyyy}", Convert.ToDateTime(idate));
            }
            else
            {
                return "";
            }
        }

       
    }
}