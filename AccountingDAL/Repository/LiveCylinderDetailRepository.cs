using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public class LiveCylinderDetailRepository : ILiveCylinderDetailRepository
    {
        public honeysaleEntities context = null;

        public LiveCylinderDetailRepository(honeysaleEntities context)
        {
            this.context = context;
        }

        public IEnumerable<LiveCylinderDetail> GetAllLiveCylinderDetail()
        {
            return (from conte in context.LiveCylinderDetails.ToList()
                    join con in context.CylinderMasters on conte.cylinder_Id equals con.ID
                    select new LiveCylinderDetail()
                    {
                        cylinder_Id = conte.cylinder_Id,
                        CylinderTypeName = con.cylinderType,
                        FilledCylinderCount = conte.FilledCylinderCount,
                        EmptyCylinderCount = conte.EmptyCylinderCount,
                        ReplacementCylinderCount = conte.ReplacementCylinderCount

                    }).ToList();

        }


    }
}
