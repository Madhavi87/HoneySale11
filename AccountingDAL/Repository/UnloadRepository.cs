using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public class UnloadRepository : IUnload
    {
        public honeysaleEntities context = null;

        public UnloadRepository(honeysaleEntities context)
        {
            this.context = context;
        }

        public IEnumerable<UnloadCylinder> GetAllUnloadCylinders()
        {
            return context.UnloadCylinders.ToList();
        }

        public UnloadCylinder GetUnloadCylinderByID(int id)
        {
            return context.UnloadCylinders.Where(obj => obj.ID == id).FirstOrDefault();
        }

        public IEnumerable<tbluser> GetAllUsers()
        {
            return context.tblusers.Where(obj => obj.role_id == 2);
        }
        public IEnumerable<CylinderMaster> GetCylinderType()
        {
            return context.CylinderMasters.ToList();
        }

        public IEnumerable<UnloadDetail> GetUnloadDetail(int id)
        {
            return context.UnloadDetails.Where(obj => obj.unload_Id == id).ToList();
        }

        public int Insert(UnloadCylinder obj)
        {

            UnloadCylinder newUnloadCylinder = new UnloadCylinder();


            newUnloadCylinder.UnloadDetail = obj.UnloadDetail;
            newUnloadCylinder.user_id = obj.user_id;
            newUnloadCylinder.userName = obj.userName;
            newUnloadCylinder.UnloadDate = obj.UnloadDate;
            newUnloadCylinder.EmptyCylinder = obj.EmptyCylinder;
            newUnloadCylinder.createdBy = obj.createdBy;
            newUnloadCylinder.createdOn = obj.createdOn;
            context.UnloadCylinders.Add(newUnloadCylinder);
            context.SaveChanges();
            if (obj.UnloadDetail != null)
            {
                foreach (var i in obj.UnloadDetail)
                {
                    UnloadDetail newUnloadDetail = new UnloadDetail();
                    newUnloadDetail.cylinder_Id = i.cylinder_Id;
                    newUnloadDetail.cylinderType = i.cylinderType;
                    newUnloadDetail.EmptyCylinder = i.EmptyCylinder;
                    newUnloadDetail.FilledCylinder = i.FilledCylinder;
                    newUnloadDetail.RejectedCylinder = i.RejectedCylinder;
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
                    newUnloadDetail.unload_Id = newUnloadCylinder.ID;
                    context.UnloadDetails.Add(newUnloadDetail);


                    var deliverycount = (
                           from delivery in context.DeliveryDetails
                           where delivery.userid == obj.user_id && delivery.C_deliveryDate == obj.UnloadDate
                           && delivery.unload_Flag == false
                           select delivery
                           ).FirstOrDefault();
                    if (deliverycount != null)
                    {
                        deliverycount.unload_Flag = true;
                        context.SaveChanges();
                    }
                }
            }
            return newUnloadCylinder.ID;
        }

        public void Update(UnloadCylinder obj)
        {
            UnloadCylinder newUnloadCylinder = context.UnloadCylinders.Where(obj1 => obj1.ID == obj.ID).FirstOrDefault();

            newUnloadCylinder.user_id = obj.user_id;
            newUnloadCylinder.userName = obj.userName;
            newUnloadCylinder.UnloadDate = obj.UnloadDate;
            newUnloadCylinder.EmptyCylinder = obj.EmptyCylinder;
            context.SaveChanges();

            if (obj.UnloadDetailForEdit != null)
            {
                foreach (var i in obj.UnloadDetailForEdit)
                {
                    UnloadDetail newUnloadDetail = new UnloadDetail();

                    newUnloadDetail.cylinder_Id = i.cylinder_Id;
                    newUnloadDetail.cylinderType = i.cylinderType;
                    newUnloadDetail.EmptyCylinder = i.EmptyCylinder;
                    newUnloadDetail.FilledCylinder = i.FilledCylinder;
                    newUnloadDetail.RejectedCylinder = i.RejectedCylinder;
                    var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == i.cylinder_Id);
                    var livecylinder = t.First();
                    if (t != null && i.ID <= 0)
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
                    //var livecylinder = context.LiveCylinderDetails.Where(c => c.cylinder_Id == i.cylinder_Id).FirstOrDefault();
                    //if (livecylinder.FilledCylinderCount == null) { livecylinder.FilledCylinderCount = 0; }
                    //if (livecylinder.EmptyCylinderCount == null) { livecylinder.EmptyCylinderCount = 0; }
                    //if (livecylinder.ReplacementCylinderCount == null) { livecylinder.ReplacementCylinderCount = 0; }
                    //if (livecylinder != null)
                    //{
                    //    livecylinder.FilledCylinderCount = livecylinder.FilledCylinderCount + i.FilledCylinder;
                    //    livecylinder.EmptyCylinderCount = livecylinder.EmptyCylinderCount + i.EmptyCylinder;
                    //    livecylinder.ReplacementCylinderCount = livecylinder.ReplacementCylinderCount + i.RejectedCylinder;
                    //    context.SaveChanges();
                    //}
                    //else
                    //{
                    //    var livecylinderNew = new LiveCylinderDetail();
                    //    livecylinderNew.cylinder_Id = i.cylinder_Id;
                    //    livecylinderNew.CylinderTypeName = i.cylinderType;
                    //    livecylinderNew.FilledCylinderCount = i.FilledCylinder;
                    //    livecylinderNew.EmptyCylinderCount = i.EmptyCylinder;
                    //    livecylinderNew.ReplacementCylinderCount = i.RejectedCylinder;
                    //    context.LiveCylinderDetails.Add(livecylinderNew);
                    //    context.SaveChanges();
                    //}
                    newUnloadDetail.unload_Id = obj.ID;
                    context.UnloadDetails.Add(newUnloadDetail);
                    context.SaveChanges();

                }
            }
        }

        public void Delete(int id)
        {
            UnloadCylinder newUnloadCylinder = context.UnloadCylinders.Where(obj => obj.ID == id).FirstOrDefault();
            var UD = context.UnloadDetails.Where(obj => obj.unload_Id == id).ToList();
            foreach (var i in UD)
            {
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
            }
            UD.ForEach(obj => context.UnloadDetails.Remove(obj));
            context.UnloadCylinders.Remove(newUnloadCylinder);
            context.SaveChanges();
        }

        public void DeleteDetail(int id)
        {
            UnloadDetail newUnloadCylinder = context.UnloadDetails.Where(obj => obj.ID == id).FirstOrDefault();
            var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == newUnloadCylinder.cylinder_Id);
            var livecylinder = t.First();
            if (t != null)
            {
                var fillecount = Convert.ToInt32(livecylinder.FilledCylinderCount) - newUnloadCylinder.FilledCylinder;
                if (fillecount <= 0)
                    livecylinder.FilledCylinderCount = 0;
                else
                    livecylinder.FilledCylinderCount = fillecount;


                var emptycount = Convert.ToInt32(livecylinder.EmptyCylinderCount) - newUnloadCylinder.EmptyCylinder;
                if (emptycount <= 0)
                    livecylinder.EmptyCylinderCount = 0;
                else
                    livecylinder.EmptyCylinderCount = emptycount;

                var replacecount = Convert.ToInt32(livecylinder.ReplacementCylinderCount) - newUnloadCylinder.RejectedCylinder;
                if (replacecount <= 0)
                    livecylinder.ReplacementCylinderCount = 0;
                else
                    livecylinder.ReplacementCylinderCount = replacecount;
                context.SaveChanges();
            }

            context.UnloadDetails.Remove(newUnloadCylinder);
            context.SaveChanges();
        }
    }
}
