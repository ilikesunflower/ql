using CMS.Extensions.Notification;
using CMS.Hubs;
using CMS.Models;
using CMS_Access.Repositories;
using CMS_Lib.DI;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Services.Notification
{
    public interface INotificationService : IScoped
    {
        void SendNotification(UserInfo user, List<int> listUserReceive, string tagEvent, CMS_EF.Models.Notification notification);

        void SendNotificationEventBySystem(UserInfo user, List<int> listUserReceive, string tagEvent, CMS_EF.Models.Notification notification);

        void SendToastNotificationRateUser(List<int> listUserReceive, string tagEvent, string title, object detail, string link);

        IQueryable<NotificationUserExtend> FindAllByReceiveId(int receiveId);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _iNotificationRepository;
        private readonly IHubContext<NotificationHub> _messageHubContext;
        public readonly INotificationBackgroundTaskQueue _backgroundTaskQueue;

        public NotificationService(INotificationBackgroundTaskQueue backgroundTaskQueue, INotificationRepository iNotificationRepository, IHubContext<NotificationHub> messageHubContext)
        {
            this._iNotificationRepository = iNotificationRepository;
            this._messageHubContext = messageHubContext;
            this._backgroundTaskQueue = backgroundTaskQueue;
        }

        public void SendNotification(UserInfo user, List<int> listUserReceive, string tagEvent, CMS_EF.Models.Notification notification)
        {
            if (user.UserId > 0)
            {
                var rs = this._iNotificationRepository.Create(notification, listUserReceive);
                if (rs != null && listUserReceive.Count > 0)
                {
                    foreach (var item in rs.NotificationUsers)
                    {
                        _backgroundTaskQueue.QueueBackgroundWorkItem((serviceScopeFactory, token) =>
                        {
                            NotificationUserExtend data = new NotificationUserExtend()
                            {
                                SenderTime = notification.SenderTime,
                                ReceiveId = item.ReceiveId,
                                IsUnread = 0,
                                Title = notification.Title,
                                Link = notification.Link,
                                Detail = notification.Detail,
                                SenderId = user.UserId,
                                SenderName = user.UserName,
                                Id = item.Id
                            };
                            this._messageHubContext.Clients.Groups(item.ReceiveId.ToString()).SendAsync(tagEvent, user.UserName, data, cancellationToken: token);
                            return Task.CompletedTask;
                        });
                    }
                }
            }
        }

        public void SendNotificationEventBySystem(UserInfo user, List<int> listUserReceive, string tagEvent, CMS_EF.Models.Notification notification)
        {
            var rs = this._iNotificationRepository.Create(notification, listUserReceive);
            if (rs != null && listUserReceive.Count > 0)
            {
                foreach (var item in rs.NotificationUsers)
                {
                    _backgroundTaskQueue.QueueBackgroundWorkItem((serviceScopeFactory, token) =>
                    {
                        NotificationUserExtend data = new NotificationUserExtend()
                        {
                            Link = notification.Link,
                            SenderTime = notification.SenderTime,
                            ReceiveId = user.UserId,
                            IsUnread = 0,
                            Title = notification.Title,
                            Detail = notification.Detail,
                            SenderId = 0,
                            SenderName = "Hệ thống",
                            Id = item.Id
                        };
                        this._messageHubContext.Clients.Groups(item.ReceiveId.ToString()).SendAsync(tagEvent, user.UserName, data, cancellationToken: token);
                        return Task.CompletedTask;
                    });
                }
            }
        }

        public void SendToastNotificationRateUser(List<int> listUserReceive, string tagEvent, string title, object detail, string link)
        {
            if (listUserReceive.Count > 0)
            {
                DateTime t = DateTime.Now;
                foreach (var item in listUserReceive)
                {
                    _backgroundTaskQueue.QueueBackgroundWorkItem((serviceScopeFactory, token) =>
                    {
                        var data = new
                        {
                            Title = "",
                            Detail = detail,
                            Link = link,
                            SenderTime = t,
                        };
                        _messageHubContext.Clients.Groups(item.ToString()).SendAsync(tagEvent, "Hệ thống", data, cancellationToken: token);
                        return Task.CompletedTask;
                    });
                }
            }
        }

        public IQueryable<NotificationUserExtend> FindAllByReceiveId(int receiveId)
        {
            return this._iNotificationRepository.FindAllByReceiveId(receiveId);
        }
    }
}
