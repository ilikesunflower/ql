using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS_Access.Repositories;

namespace CMS.Areas.Admin.ViewModels.Notification
{
    public class UserNotificationViewModel
    {
        public ReflectionIT.Mvc.Paging.PagingList<NotificationUserExtend> ListData { set; get; }

        public int PageSize { get; set; }
    }
}
