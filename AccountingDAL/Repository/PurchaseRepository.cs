using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public class PurchaseRepository : IPurchase
    {
        public honeysaleEntities context = null;

        public PurchaseRepository(honeysaleEntities context)
        {
            this.context = context;
        }

        public IEnumerable<PurchaseCylinder> GetAllPurchaseCylinder()
        {
            return context.PurchaseCylinders.ToList();
        }
        public IEnumerable<CylinderMaster> GetCylinderType()
        {
            return context.CylinderMasters.ToList();
        }

        public IEnumerable<PurchaseDetail> GetPurchaseDetail(int id)
        {
            return context.PurchaseDetails.Where(obj => obj.purchase_Id == id).ToList();
        }
      
        public PurchaseCylinder GetPurchaseByID(int id)
        {
            return context.PurchaseCylinders.Where(obj => obj.ID == id).FirstOrDefault();
        }

        public int Insert(PurchaseCylinder obj)
        {

            PurchaseCylinder newPurchaseMaster = new PurchaseCylinder();
          
           // LiveCylinderDetail livecylinder = new LiveCylinderDetail();
            newPurchaseMaster.PurchaseDetail = obj.PurchaseDetail;
            newPurchaseMaster.BillNo = obj.BillNo;
            newPurchaseMaster.dealerName = obj.dealerName;
            newPurchaseMaster.purchaseDate = obj.purchaseDate;
            newPurchaseMaster.amount = obj.amount;
            context.PurchaseCylinders.Add(newPurchaseMaster);
            context.SaveChanges();
            if (obj.PurchaseDetail != null)
            {
                foreach (var i in obj.PurchaseDetail)
                {
                    PurchaseDetail newpurchaseDetail = new PurchaseDetail();
                    newpurchaseDetail.cylinder_Id = i.cylinder_Id;
                    newpurchaseDetail.cylinderType = i.cylinderType;
                    newpurchaseDetail.FilledCylinder = i.FilledCylinder;
                    var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == i.cylinder_Id);
                    var livecylinder = t.First();
                    if (t != null)
                    {
                        livecylinder.FilledCylinderCount = Convert.ToInt32(livecylinder.FilledCylinderCount) + i.FilledCylinder;
                        context.SaveChanges();
                    }
                    //var livecylinder = context.LiveCylinderDetails.Where(c => c.cylinder_Id == i.cylinder_Id).FirstOrDefault();
                    //if (livecylinder != null)
                    //{
                    //   livecylinder.FilledCylinderCount= livecylinder.FilledCylinderCount + i.FilledCylinder;
                    //   context.SaveChanges();
                    //}
                    //else
                    //{
                    //    var livecylinderNew = new LiveCylinderDetail();
                    //    livecylinderNew.cylinder_Id = i.cylinder_Id;
                    //    livecylinderNew.CylinderTypeName = i.cylinderType;
                    //    livecylinderNew.FilledCylinderCount = i.FilledCylinder;
                    //    livecylinderNew.EmptyCylinderCount = 0;
                    //    livecylinderNew.ReplacementCylinderCount = 0;
                    //    context.LiveCylinderDetails.Add(livecylinderNew);
                    //    context.SaveChanges();
                    //}
                    newpurchaseDetail.purchase_Id = newPurchaseMaster.ID;
                    context.PurchaseDetails.Add(newpurchaseDetail);
                    context.SaveChanges();
                }
            }
            return newPurchaseMaster.ID;
        }

        public void Update(PurchaseCylinder obj)
        {
            PurchaseCylinder newPurchaseMaster = context.PurchaseCylinders.Where(obj1 => obj1.ID == obj.ID).FirstOrDefault();
            PurchaseDetail newpurchaseDetail = new PurchaseDetail();

            newPurchaseMaster.dealerName = obj.dealerName;
            newPurchaseMaster.BillNo = obj.BillNo;
            newPurchaseMaster.purchaseDate = obj.purchaseDate;
            newPurchaseMaster.amount = obj.amount;
            context.SaveChanges();
            if(obj.PurchaseDetailForEdit != null)
            {
                foreach (var i in obj.PurchaseDetailForEdit)
                {
                    newpurchaseDetail.cylinder_Id = i.cylinder_Id;
                    newpurchaseDetail.cylinderType = i.cylinderType;
                    newpurchaseDetail.FilledCylinder = i.FilledCylinder;
                    var livecylinder = context.LiveCylinderDetails.Where(c => c.cylinder_Id == i.cylinder_Id).FirstOrDefault();
                    if (livecylinder != null)
                    {
                        livecylinder.FilledCylinderCount = livecylinder.FilledCylinderCount + i.FilledCylinder;
                        context.SaveChanges();
                    }
                    else
                    {
                        var livecylinderNew = new LiveCylinderDetail();
                        livecylinderNew.cylinder_Id = i.cylinder_Id;
                        livecylinderNew.CylinderTypeName = i.cylinderType;
                        livecylinderNew.FilledCylinderCount = i.FilledCylinder;
                        livecylinderNew.EmptyCylinderCount = 0;
                        livecylinderNew.ReplacementCylinderCount = 0;
                        context.LiveCylinderDetails.Add(livecylinderNew);
                        context.SaveChanges();
                    }
                    newpurchaseDetail.purchase_Id = obj.ID;
                    context.PurchaseDetails.Add(newpurchaseDetail);
                    context.SaveChanges();
                }
            }
        }

       
        public void Delete(int id)
        {
            PurchaseCylinder newPurchaseMaster = context.PurchaseCylinders.Where(obj => obj.ID == id).FirstOrDefault();
            var PurchaseDetail = context.PurchaseDetails.Where(obj => obj.purchase_Id == id).ToList();
            foreach (var temp in PurchaseDetail)
            {
                var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == temp.cylinder_Id);
                var livecylinder = t.First();
                if (t != null)
                {
                    livecylinder.FilledCylinderCount = livecylinder.FilledCylinderCount - temp.FilledCylinder;
                    context.SaveChanges();
                }
            }
            PurchaseDetail.ForEach(obj => context.PurchaseDetails.Remove(obj));
            context.PurchaseCylinders.Remove(newPurchaseMaster);
            context.SaveChanges();
        }
        public void DeleteDetail(int id)
        {
            PurchaseDetail newPurchaseCylinder = context.PurchaseDetails.Where(obj => obj.ID == id).FirstOrDefault();
            var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == newPurchaseCylinder.cylinder_Id);
            var livecylinder = t.First();
            if (t != null)
            {
                livecylinder.FilledCylinderCount = livecylinder.FilledCylinderCount - newPurchaseCylinder.FilledCylinder;
                context.SaveChanges();
            }
            context.PurchaseDetails.Remove(newPurchaseCylinder);
            context.SaveChanges();
        }
    }
}
