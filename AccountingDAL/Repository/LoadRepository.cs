using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoneySaleDAL
{
    public class LoadRepository : ILoad
    {
        public honeysaleEntities context = null;

        public LoadRepository(honeysaleEntities context)
        {
            this.context = context;
        }

        public IEnumerable<LoadCylinder> GetAllLoadCylinders()
        {
            return context.LoadCylinders.OrderByDescending(ite => ite.createdOn).ToList();
        }

        public LoadCylinder GetLoadCylinderByID(int id)
        {
            return context.LoadCylinders.Where(obj => obj.ID == id).FirstOrDefault();
        }

        public IEnumerable<tbluser> GetAllUsers()
        {
            return context.tblusers.Where(obj => obj.role_id == 2);
        }
        public IEnumerable<CylinderMaster> GetCylinderType()
        {
            return context.CylinderMasters.ToList();
        }

        public IEnumerable<LoadDetail> GetLoadDetail(int id)
        {
            return context.LoadDetails.Where(obj => obj.load_Id == id).ToList();
        }


        public int Insert(LoadCylinder obj)
        {

            LoadCylinder newLoadMaster = new LoadCylinder();


            newLoadMaster.user_id = obj.user_id;
            newLoadMaster.userName = obj.userName;
            newLoadMaster.loadDate = obj.loadDate;
            newLoadMaster.createdBy = obj.createdBy;
            newLoadMaster.createdOn = obj.createdOn;
            context.LoadCylinders.Add(newLoadMaster);
            context.SaveChanges();
            if (obj.LoadDetail != null)
            {
                foreach (var i in obj.LoadDetail)
                {
                    LoadDetail newLoadDetail = new LoadDetail();
                    newLoadDetail.cylinder_Id = i.cylinder_Id;
                    newLoadDetail.cylinderType = i.cylinderType;
                    newLoadDetail.FilledCylinder = i.FilledCylinder;
                    var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == i.cylinder_Id);
                    var livecylinder = t.First();
                    if (t != null)
                    {
                        //livecylinder.FilledCylinderCount = livecylinder.FilledCylinderCount - i.FilledCylinder;
                        //context.SaveChanges();
                        var fillecount = Convert.ToInt32(livecylinder.FilledCylinderCount) - i.FilledCylinder;
                        if (fillecount <= 0)
                            livecylinder.FilledCylinderCount = 0;
                        else
                            livecylinder.FilledCylinderCount = fillecount;
                        context.SaveChanges();
                    }
                    
                    newLoadDetail.load_Id = newLoadMaster.ID;
                    context.LoadDetails.Add(newLoadDetail);
                    context.SaveChanges();
                }
            }
            return newLoadMaster.ID;
        }

        public void Update(LoadCylinder obj)
        {
            LoadCylinder newLoadMaster = context.LoadCylinders.Where(obj1 => obj1.ID == obj.ID).FirstOrDefault();


            newLoadMaster.user_id = obj.user_id;
            newLoadMaster.userName = obj.userName;
            newLoadMaster.loadDate = obj.loadDate;
            context.SaveChanges();
            if (obj.LoadDetailForEdit != null)
            {
                foreach (var i in obj.LoadDetailForEdit)
                {
                    LoadDetail newLoadDetail = new LoadDetail();
                    newLoadDetail.cylinder_Id = i.cylinder_Id;
                    newLoadDetail.cylinderType = i.cylinderType;
                    newLoadDetail.FilledCylinder = i.FilledCylinder;
                    var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == i.cylinder_Id);
                    var livecylinder = t.First();
                    if (t != null && i.ID <= 0)
                    {
                        //livecylinder.FilledCylinderCount = livecylinder.FilledCylinderCount - i.FilledCylinder;
                        //context.SaveChanges();
                        var fillecount = Convert.ToInt32(livecylinder.FilledCylinderCount) - i.FilledCylinder;
                        if (fillecount <= 0)
                            livecylinder.FilledCylinderCount = 0;
                        else
                            livecylinder.FilledCylinderCount = fillecount;
                        context.SaveChanges();
                    }

                    // var livecylinder = context.LiveCylinderDetails.Where(c => c.cylinder_Id == i.cylinder_Id).FirstOrDefault();
                    // var livecylinder = t.First();
                    //if (livecylinder != null)
                    //{
                    //    livecylinder.FilledCylinderCount = livecylinder.FilledCylinderCount - i.FilledCylinder;
                    //    context.SaveChanges();
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
                    newLoadDetail.load_Id = obj.ID;
                    context.LoadDetails.Add(newLoadDetail);
                    context.SaveChanges();

                }
            }
        }

        public void Delete(int id)
        {
            LoadCylinder newLoadCylinder = context.LoadCylinders.Where(obj => obj.ID == id).FirstOrDefault();

            var ld = context.LoadDetails.Where(obj => obj.load_Id == id).ToList();
            foreach (var i in ld)
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
                    context.SaveChanges();
                }
            }

            context.LoadDetails.Where(obj => obj.load_Id == id).ToList().ForEach(obj => context.LoadDetails.Remove(obj));
            context.LoadCylinders.Remove(newLoadCylinder);
            context.SaveChanges();
        }
        public void DeleteDetail(int id)
        {
            LoadDetail newLoadCylinder = context.LoadDetails.Where(obj => obj.ID == id).FirstOrDefault();
            var t = context.LiveCylinderDetails.Where(c => c.cylinder_Id == newLoadCylinder.cylinder_Id);
            var livecylinder = t.First();
            if (t != null)
            {

                var fillecount = Convert.ToInt32(livecylinder.FilledCylinderCount) + newLoadCylinder.FilledCylinder;
                if (fillecount <= 0)
                    livecylinder.FilledCylinderCount = 0;
                else
                    livecylinder.FilledCylinderCount = fillecount;
                context.SaveChanges();
            }

            context.LoadDetails.Remove(newLoadCylinder);
            context.SaveChanges();
        }
    }
}
