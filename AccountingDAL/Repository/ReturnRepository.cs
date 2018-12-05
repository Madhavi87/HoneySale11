using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public class ReturnRepository : IReturn
    {
        public honeysaleEntities context = null;

        public ReturnRepository(honeysaleEntities context)
        {
            this.context = context;
        }

        public IEnumerable<ReturnCylinder> GetAllReturns()
        {
            return context.ReturnCylinders.ToList();
        }

        public ReturnCylinder GetReturnByID(int id)
        {
            return context.ReturnCylinders.Where(obj => obj.ID == id).FirstOrDefault();
        }
        public IEnumerable<CylinderMaster> GetCylinderType()
        {
            return context.CylinderMasters.ToList();
        }

        public IEnumerable<ReturnDetail> GetReturnDetail(int id)
        {
            return context.ReturnDetails.Where(obj => obj.return_Id == id).ToList();
        }

        public int Insert(ReturnCylinder obj)
        {

            ReturnCylinder newReturnMaster = new ReturnCylinder();


            newReturnMaster.ERVNo = obj.ERVNo;
            newReturnMaster.dealerName = obj.dealerName;
            newReturnMaster.returnDate = obj.returnDate;
            context.ReturnCylinders.Add(newReturnMaster);
            context.SaveChanges();
            if (obj.ReturnDetail != null)
            {

                foreach (var i in obj.ReturnDetail)
                {
                    ReturnDetail newReturnDetail = new ReturnDetail();
                    newReturnDetail.cylinder_Id = i.cylinder_Id;
                    newReturnDetail.cylinderType = i.cylinderType;
                    newReturnDetail.FilledCylinder = i.FilledCylinder;
                    newReturnDetail.EmptyCylinder = i.EmptyCylinder;
                    newReturnDetail.RejectedCylinder = i.RejectedCylinder;
                    var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == i.cylinder_Id);
                    var livecylinder = t.First();

                    if (t != null)
                    {
                        var fillecount = Convert.ToInt32(livecylinder.FilledCylinderCount) - i.FilledCylinder;
                        if (fillecount <= 0)
                            livecylinder.FilledCylinderCount = 0;
                        else
                            livecylinder.FilledCylinderCount = fillecount;


                        var emptycount = Convert.ToInt32(livecylinder.EmptyCylinderCount) - i.EmptyCylinder;
                        if (emptycount <= 0)
                            livecylinder.EmptyCylinderCount = 0;
                        else
                            livecylinder.EmptyCylinderCount = emptycount;

                        var replacecount = Convert.ToInt32(livecylinder.ReplacementCylinderCount) - i.RejectedCylinder;
                        if (replacecount <= 0)
                            livecylinder.ReplacementCylinderCount = 0;
                        else
                            livecylinder.ReplacementCylinderCount = replacecount;

                        context.SaveChanges();
                    }
                    newReturnDetail.return_Id = newReturnMaster.ID;
                    context.ReturnDetails.Add(newReturnDetail);
                    context.SaveChanges();
                }
            }
            return newReturnMaster.ID;
        }

        public void Update(ReturnCylinder obj)
        {
            ReturnCylinder newReturnMaster = context.ReturnCylinders.Where(obj1 => obj1.ID == obj.ID).FirstOrDefault();

            newReturnMaster.ERVNo = obj.ERVNo;
            newReturnMaster.dealerName = obj.dealerName;
            newReturnMaster.returnDate = obj.returnDate;
            context.SaveChanges();
            if (obj.ReturnDetailForEdit != null)
            {
                foreach (var i in obj.ReturnDetailForEdit)
                {
                    ReturnDetail newReturnDetail = new ReturnDetail();
                    newReturnDetail.cylinder_Id = i.cylinder_Id;
                    newReturnDetail.cylinderType = i.cylinderType;
                    newReturnDetail.FilledCylinder = i.FilledCylinder;
                    newReturnDetail.EmptyCylinder = i.EmptyCylinder;
                    newReturnDetail.RejectedCylinder = i.RejectedCylinder;
                    var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == i.cylinder_Id);
                    var livecylinder = t.First();

                    if (t != null)
                    {
                        var fillecount = Convert.ToInt32(livecylinder.FilledCylinderCount) - i.FilledCylinder;
                        if (fillecount <= 0)
                            livecylinder.FilledCylinderCount = 0;
                        else
                            livecylinder.FilledCylinderCount = fillecount;


                        var emptycount = Convert.ToInt32(livecylinder.EmptyCylinderCount) - i.EmptyCylinder;
                        if (emptycount <= 0)
                            livecylinder.EmptyCylinderCount = 0;
                        else
                            livecylinder.EmptyCylinderCount = emptycount;

                        var replacecount = Convert.ToInt32(livecylinder.ReplacementCylinderCount) - i.RejectedCylinder;
                        if (replacecount <= 0)
                            livecylinder.ReplacementCylinderCount = 0;
                        else
                            livecylinder.ReplacementCylinderCount = replacecount;

                        context.SaveChanges();
                    }
                    newReturnDetail.return_Id = obj.ID;
                    context.ReturnDetails.Add(newReturnDetail);
                    context.SaveChanges();
                }
            }
        }

        public void Delete(int id)
        {
            ReturnCylinder newReturnMaster = context.ReturnCylinders.Where(obj => obj.ID == id).FirstOrDefault();
            var RD = context.ReturnDetails.Where(obj => obj.return_Id == id).ToList();
            foreach (var i in RD)
            {
                var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == i.cylinder_Id);
                var livecylinder = t.First();

                if (t != null)
                {
                    var fillecount = Convert.ToInt32(livecylinder.FilledCylinderCount) + i.FilledCylinder;
                    if (fillecount <= 0)
                        livecylinder.FilledCylinderCount = 0;
                    else
                        livecylinder.FilledCylinderCount = fillecount;


                    var emptycount = Convert.ToInt32(livecylinder.EmptyCylinderCount) + i.EmptyCylinder;
                    if (emptycount <= 0)
                        livecylinder.EmptyCylinderCount = 0;
                    else
                        livecylinder.EmptyCylinderCount = emptycount;

                    var replacecount = Convert.ToInt32(livecylinder.ReplacementCylinderCount) + i.RejectedCylinder;
                    if (replacecount <= 0)
                        livecylinder.ReplacementCylinderCount = 0;
                    else
                        livecylinder.ReplacementCylinderCount = replacecount;

                    context.SaveChanges();
                }
            }
            RD.ForEach(obj => context.ReturnDetails.Remove(obj));
            context.ReturnCylinders.Remove(newReturnMaster);
            context.SaveChanges();
        }
        public void DeleteDetail(int id)
        {
            ReturnDetail newReturnCylinder = context.ReturnDetails.Where(obj => obj.ID == id).FirstOrDefault();
            var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == newReturnCylinder.cylinder_Id);
            var livecylinder = t.First();

            if (t != null)
            {
                var fillecount = Convert.ToInt32(livecylinder.FilledCylinderCount) + newReturnCylinder.FilledCylinder;
                if (fillecount <= 0)
                    livecylinder.FilledCylinderCount = 0;
                else
                    livecylinder.FilledCylinderCount = fillecount;


                var emptycount = Convert.ToInt32(livecylinder.EmptyCylinderCount) + newReturnCylinder.EmptyCylinder;
                if (emptycount <= 0)
                    livecylinder.EmptyCylinderCount = 0;
                else
                    livecylinder.EmptyCylinderCount = emptycount;

                var replacecount = Convert.ToInt32(livecylinder.ReplacementCylinderCount) + newReturnCylinder.RejectedCylinder;
                if (replacecount <= 0)
                    livecylinder.ReplacementCylinderCount = 0;
                else
                    livecylinder.ReplacementCylinderCount = replacecount;

                context.SaveChanges();
            }
            context.ReturnDetails.Remove(newReturnCylinder);
            context.SaveChanges();
        }
    }
}
