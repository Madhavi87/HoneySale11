using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public interface ILiveCylinderDetailRepository
    {
        IEnumerable<LiveCylinderDetail> GetAllLiveCylinderDetail();
        

    }
}
