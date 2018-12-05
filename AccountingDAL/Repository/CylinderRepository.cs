using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public class CylinderRepository : ICylinderRepository
    {
        public honeysaleEntities context = null;

        public CylinderRepository(honeysaleEntities context)
        {
            this.context = context;
        }

        public IEnumerable<CylinderMaster> GetAllCylinders()
        {
            return context.CylinderMasters.ToList();
        }

        public CylinderMaster GetCylinderByID(int id)
        {
            return context.CylinderMasters.Where(obj => obj.ID == id).FirstOrDefault();
        }

        public int Insert(CylinderMaster obj)
        {

            CylinderMaster newCustomerMaster = new CylinderMaster();
            if (context.CylinderMasters.ToList() != null && context.CylinderMasters.ToList().Count > 0)
                newCustomerMaster.ID = Convert.ToInt32(context.CylinderMasters.ToList().Last().ID) + 1;
            else
                newCustomerMaster.ID = 1;

            // newCustomerMaster.ID = Convert.ToInt32(id.ID) + 1;
            newCustomerMaster.amount = Convert.ToDecimal(string.Format("{0:0.00}", obj.amount));
            newCustomerMaster.cylinderType = obj.cylinderType;
            newCustomerMaster.openingEmpty = obj.openingEmpty;
            newCustomerMaster.openingFilled = obj.openingFilled;
            newCustomerMaster.openingReplace = obj.openingReplace;
            newCustomerMaster.CreatedOn = DateTime.Now;
            newCustomerMaster.CreatedBy = obj.CreatedBy;


            context.CylinderMasters.Add(newCustomerMaster);
            context.SaveChanges();

            var livecylinderNew = new LiveCylinderDetail();
            livecylinderNew.cylinder_Id = newCustomerMaster.ID;
            livecylinderNew.CylinderTypeName = newCustomerMaster.cylinderType;
            livecylinderNew.FilledCylinderCount = newCustomerMaster.openingFilled;
            livecylinderNew.EmptyCylinderCount = newCustomerMaster.openingEmpty;
            livecylinderNew.ReplacementCylinderCount = newCustomerMaster.openingReplace;
            context.LiveCylinderDetails.Add(livecylinderNew);
            context.SaveChanges();

            return newCustomerMaster.ID;
        }

        public void Update(CylinderMaster obj)
        {
            CylinderMaster newCustomerMaster = context.CylinderMasters.Where(obj1 => obj1.ID == obj.ID).FirstOrDefault();
            newCustomerMaster.amount = Convert.ToDecimal(string.Format("{0:0.00}", obj.amount));
            newCustomerMaster.cylinderType = obj.cylinderType;
            newCustomerMaster.ModifiedBy = obj.ModifiedBy;
            newCustomerMaster.ModifiedOn = DateTime.Now;
            newCustomerMaster.openingEmpty = obj.openingEmpty;
            newCustomerMaster.openingFilled = obj.openingFilled;
            newCustomerMaster.openingReplace = obj.openingReplace;
            context.SaveChanges();
            var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == newCustomerMaster.ID);

            //if (t != null)
            //{
            //    var livecylinder = t.First();
            //    if (livecylinder.FilledCylinderCount == null) { livecylinder.FilledCylinderCount = 0; }
            //    if (livecylinder.EmptyCylinderCount == null) { livecylinder.EmptyCylinderCount = 0; }
            //    if (livecylinder.ReplacementCylinderCount == null) { livecylinder.ReplacementCylinderCount = 0; }
            //    livecylinder.FilledCylinderCount = livecylinder.FilledCylinderCount + obj.openingFilled;
            //    livecylinder.EmptyCylinderCount = livecylinder.EmptyCylinderCount + obj.openingEmpty;
            //    livecylinder.ReplacementCylinderCount = livecylinder.FilledCylinderCount + obj.openingReplace;
            //    context.SaveChanges();
            //}
            //else
            //{
            //    var livecylinder = new LiveCylinderDetail();
            //    livecylinder.cylinder_Id = newCustomerMaster.ID;
            //    livecylinder.CylinderTypeName = newCustomerMaster.cylinderType;
            //    livecylinder.FilledCylinderCount = obj.openingFilled;
            //    livecylinder.EmptyCylinderCount = obj.openingEmpty;
            //    livecylinder.ReplacementCylinderCount = obj.openingReplace;
            //    context.LiveCylinderDetails.Add(livecylinder);
            //    context.SaveChanges();
            //}
        }

        public void Delete(int id)
        {
            CylinderMaster newCustomerMaster = context.CylinderMasters.Where(obj => obj.ID == id).FirstOrDefault();
            context.CylinderMasters.Remove(newCustomerMaster);
            context.SaveChanges();
        }
    }
}
