using CMS_EF.DbContext;
using CMS_EF.Models;
using CMS_Lib.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CMS_Access.Repositories
{
    public interface INotificationRepository : IBaseRepository<Notification>, IScoped
    {
        IQueryable<NotificationUserExtend> FindAllByReceiveId(int receiveId);

        NotificationUserInsert Create(Notification entity, List<int> listUser);

        NotificationUserViewTime Update(NotificationUserViewTime data);

        NotificationUserViewTime Insert(NotificationUserViewTime data);

        NotificationUserViewTime FindNotificationUserViewTimeByUserId(int userId);
    
        int NotificationUserUnReadCountTime(int userId, DateTime? lastTimeRead);

        int UpdateIsRead(int id);
        int UpdateReceiveIdIsReadAll(int userId);

        bool DeleteNotificationUser(int id);

        int DeleteAllNotificationUser(List<int> id);

    }
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext applicationDbContext, IHttpContextAccessor context) : base(applicationDbContext, context)
        {
        }

        public IQueryable<NotificationUserExtend> FindAllByReceiveId(int receiveId)
        {
            var rs = (from nu in ApplicationDbContext.NotificationUsers
                      join n in ApplicationDbContext.Notifications on new { Id = nu.NotificationId, Flag = nu.Flag } equals new
                      { Id = n.Id, Flag = n.Flag }
                      join u in base.ApplicationDbContext.Users on new { Id = nu.SenderId, Flag = nu.Flag } equals new
                      { Id = u.Id, Flag = u.Flag }
                      orderby nu.SenderTime descending
                      where nu.Flag == 0 && nu.ReceiveId == receiveId
                      select new NotificationUserExtend()
                      {
                          Id = nu.Id,
                          Title = n.Title,
                          Detail = n.Detail,
                          ReceiveId = nu.ReceiveId,
                          SenderTime = nu.SenderTime,
                          SenderId = nu.SenderId,
                          SenderName = u.UserName,
                          Link = n.Link,
                          IsUnread = nu.IsUnread,
                      });
            return rs;
        }

        public NotificationUserInsert Create(Notification entity, List<int> listUser)
        {
            if (listUser.Count > 0)
            {
                using IDbContextTransaction transaction = ApplicationDbContext.Database.BeginTransaction();
                try
                {
                    ApplicationDbContext.Notifications.Add(entity);
                    ApplicationDbContext.SaveChanges();
                    List<NotificationUser> listNotificationUsers = new List<NotificationUser>();
                    foreach (var t in listUser)
                    {
                        NotificationUser notificationUser = new NotificationUser()
                        {
                            IsUnread = 0,
                            SenderTime = entity.SenderTime,
                            NotificationId = entity.Id,
                            ReceiveId = t,
                            SenderId = entity.CreatedBy
                        };
                        listNotificationUsers.Add(notificationUser);
                    }
                    ApplicationDbContext.NotificationUsers.AddRange(listNotificationUsers);
                    ApplicationDbContext.SaveChanges();
                    transaction.Commit();
                    NotificationUserInsert rs = new NotificationUserInsert()
                    {
                        Notification = entity,
                        NotificationUsers = listNotificationUsers
                    };
                    return rs;
                }
                catch
                {
                    transaction.Rollback();
                    return null;
                }
            }

            return null;
        }

        public NotificationUserViewTime Update(NotificationUserViewTime data)
        {
            this.ApplicationDbContext.Set<NotificationUserViewTime>().Update(data);
            this.ApplicationDbContext.SaveChanges();
            return data;
        }

        public NotificationUserViewTime Insert(NotificationUserViewTime data)
        {
            this.ApplicationDbContext.Set<NotificationUserViewTime>().Add(data);
            this.ApplicationDbContext.SaveChanges();
            return data;
        }

        public NotificationUserViewTime FindNotificationUserViewTimeByUserId(int userId)
        {
            return this.ApplicationDbContext.NotificationUserViewTimes.FirstOrDefault(x => x.Id == userId);
        }

        public int NotificationUserUnReadCountTime(int userId, DateTime? lastTimeRead)
        {
            if (!lastTimeRead.HasValue)
            {
                lastTimeRead = this.ApplicationDbContext.NotificationUserViewTimes.FirstOrDefault(x => x.Id == userId)
                    ?.LastViewTime;
            }
            return this.ApplicationDbContext.NotificationUsers.Count(x =>
                x.Flag == 0 && x.IsUnread == 0 && (!lastTimeRead.HasValue || x.SenderTime > lastTimeRead.Value) && x.ReceiveId == userId);
        }

        public int UpdateIsRead(int id)
        {
            var rs = ApplicationDbContext.NotificationUsers.FirstOrDefault(x => x.Flag == 0 && x.Id == id);
            if (rs != null)
            {
                rs.IsUnread = 1;
                ApplicationDbContext.SaveChanges();
                return 1;
            }
            return 0;
        }

        public int UpdateReceiveIdIsReadAll(int userId)
        {
            var data = ApplicationDbContext.NotificationUsers.Where(x => x.ReceiveId == userId && x.IsUnread == 0 && x.Flag == 0).ToList();
            data.ForEach(x => x.IsUnread = 1);
            ApplicationDbContext.NotificationUsers.UpdateRange(data);
            ApplicationDbContext.SaveChanges();
            return data.Count;
        }

        public bool DeleteNotificationUser(int id)
        {
            var data = ApplicationDbContext.NotificationUsers.FirstOrDefault(x => x.Flag == 0 && x.Id == id);
            if (data != null)
            {
                data.Flag = -1;
                ApplicationDbContext.SaveChanges();
                return true;
            }

            return false;
        }

        public int DeleteAllNotificationUser(List<int> id)
        {
            var data = ApplicationDbContext.NotificationUsers.Where(x => id.Contains(x.Id)).ToList();
            data.ForEach(x => x.Flag = -1);
            ApplicationDbContext.NotificationUsers.UpdateRange(data);
            ApplicationDbContext.SaveChanges();
            return data.Count;
        }

      
    }

    public class NotificationUserExtend
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string SenderName { get; set; }
        public int ReceiveId { get; set; }
        public int IsUnread { get; set; }
        public DateTime SenderTime { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Link { get; set; }
    }

    public class NotificationUserInsert
    {
        public Notification Notification { get; set; }
        public List<NotificationUser> NotificationUsers { get; set; }
    }
}
