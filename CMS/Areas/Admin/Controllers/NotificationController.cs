using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Areas.Admin.ViewModels.Notification;
using CMS.Controllers;
using CMS.Models.ModelContainner;
using CMS_Access.Repositories;
using CMS_Lib.Extensions.Attribute;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;
using CMS_EF.Models;

namespace CMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Obsolete]
    [NonLoad]
    public class NotificationController : BaseController
    {
        private readonly ILogger _iLogger;
        private readonly INotificationRepository _iNotificationRepository;

        public NotificationController(ILogger<NotificationController> iLogger, INotificationRepository iNotificationRepository)
        {
            this._iLogger = iLogger;
            this._iNotificationRepository = iNotificationRepository;
        }

        [HttpGet]
        [NoActiveMenu]
        public async Task<IActionResult> UsersNotification(string txtSearch, string senderName, int? isUnread, int pageindex = 1)
        {
            var query = this._iNotificationRepository.FindAllByReceiveId(UserInfo.UserId);
            if (!string.IsNullOrEmpty(txtSearch))
            {
                query = query.Where(x => (x.Title.Contains(txtSearch) || x.Detail.Contains(txtSearch)));
            }

            if (!string.IsNullOrEmpty(senderName))
            {
                query = query.Where(x => x.SenderName.Contains(senderName));
            }
            if (isUnread.HasValue)
            {
                query = query.Where(x => x.IsUnread == isUnread);
            }
            var model = await PagingList<NotificationUserExtend>.CreateAsync(query.OrderByDescending(x => x.Id), PageSize, pageindex);
            model.RouteValue = new RouteValueDictionary
            {
                {"txtSearch", txtSearch},
                {"senderName", senderName},
                {"isUnread", isUnread}
            };
            UserNotificationViewModel rs = new UserNotificationViewModel()
            {
                ListData = model,
                PageSize = PageSize
            };
            return View(rs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu thông báo");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu thông báo, không thể xóa"
                });
            }

            var rs = this._iNotificationRepository.DeleteNotificationUser((int)id);
            if (rs)
            {
                ILoggingService.Infor(this._iLogger, "Xóa thông báo thành công:", "id:" + id);
                ToastMessage(1, "Xóa thông báo thành công");
                return Json(new
                {
                    msg = "successful",
                    content = "Xóa thông báo thành công"
                });
            }
            else
            {
                ILoggingService.Error(this._iLogger, "Xóa thông báo lỗi:", "id:" + id);
                ToastMessage(-1, "Xóa thông báo lỗi");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu thông báo, không thể xóa"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NonLoad]
        public JsonResult DeleteAll(List<int> id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu thông báo");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                int rs = this._iNotificationRepository.DeleteAllNotificationUser(id);
                if (rs >= 0)
                {
                    ToastMessage(1, $"Xóa thành công {rs} bản ghi");
                    this._iLogger.LogInformation($"Xóa thành công {rs} thông báo");
                    return Json(new
                    {
                        msg = "successful",
                        content = ""
                    });
                }
                else
                {
                    ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                    this._iLogger.LogError($"Xóa dữ liệu lỗi, liên hệ người quản trị: id {id}");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                    });
                }
            }
            catch (Exception)
            {
                ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                this._iLogger.LogError($"Xóa dữ liệu lỗi, liên hệ người quản trị: id {id}");
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ReadUserNotificationAll()
        {
            var rs = this._iNotificationRepository.UpdateReceiveIdIsReadAll(UserInfo.UserId);
            if (rs > 0)
            {
                ToastMessage(1, $"Đánh dấu đọc {rs} thông báo thành công");
                return Json(new
                {
                    msg = "successful",
                    content = $"Đánh dấu đọc {rs} thông báo thành công"
                });
            }
            else
            {
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu thông báo, không thể xóa"
                });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ReadUserNotification(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu thông báo");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu thông báo, không thể xóa"
                });
            }
            var rs = this._iNotificationRepository.UpdateIsRead(id.Value);
            if (rs > 0)
            {
                return Json(new
                {
                    msg = "successful",
                    content = $"Đánh dấu đọc {rs} thông báo thành công"
                });
            }
            else
            {
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu thông báo, không thể xóa"
                });
            }
        }

        
        [HttpGet]
        public JsonResult GetNotificationByUser()
        {
            // cập nhật thời gian khi user click vào nút thông báo
            var userTime = _iNotificationRepository.FindNotificationUserViewTimeByUserId(UserInfo.UserId);
            if (userTime == null)
            {
                NotificationUserViewTime userViewTime = new NotificationUserViewTime();
                userViewTime.Id = UserInfo.UserId;
                userViewTime.LastViewTime = DateTime.Now;
                _iNotificationRepository.Insert(userViewTime);
            }
            else
            {
                userTime.LastViewTime = DateTime.Now;
                _iNotificationRepository.Update(userTime);
            }
            // lấy ra danh sách 10 thông báo mới nhất
            List<NotificationUserExtend> listData = _iNotificationRepository.FindAllByReceiveId(UserInfo.UserId).Take(10).ToList();
            return Json(new
            {
                msg = "successful",
                content = new
                {
                    data = listData,
                    countUnRead = this._iNotificationRepository.NotificationUserUnReadCountTime(UserInfo.UserId, null)
                }
            });
        }
    }
}
