using Castle.Core.Internal;
using CMS.Controllers;
using CMS.Models.ModelContainner;
using CMS_Access.Repositories;
using CMS_EF.Models;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Obsolete]
    public class LoggingsController : BaseController
    {
        private readonly ILoggingRepository _iLoggingRepository;
        private readonly IApplicationUserRepository _iApplicationUserRepository;
        private readonly ILogger<LoggingsController> _iLogger;

        public LoggingsController(ILoggingRepository iLoggingRepository, IApplicationUserRepository iApplicationUserRepository,
            ILogger<LoggingsController> iLogger)
        {
            this._iLoggingRepository = iLoggingRepository;
            this._iApplicationUserRepository = iApplicationUserRepository;
            this._iLogger = iLogger;
        }

        // GET: Admin/Loggings
        [Authorize(Policy = "PermissionMVC")]
        public async Task<IActionResult> Index(string txtSearch, int? userId, int? type, string startTime,
            string endTime, int pageindex = 1)
        {
            var q = _iLoggingRepository.FindAll().AsNoTracking().Where(x => x.Flag == 0);
            if (!txtSearch.IsNullOrEmpty()) q = q.Where(p => EF.Functions.Like(p.Action, "%" + txtSearch.Trim() + "%") || p.UserFullName == txtSearch.Trim());
            if (userId.HasValue) q = q.Where(x => x.UserId == userId.Value);
            if (type.HasValue) q = q.Where(x => x.LogLevel == type.Value);
            if (!string.IsNullOrEmpty(startTime))
            {
                var start = DateTime.ParseExact(startTime, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture);
                q = q.Where(x => x.CreatedAt > start);
            }

            if (!string.IsNullOrEmpty(endTime))
            {
                var end = DateTime.ParseExact(endTime, "dd/MM/yyyy",
                    CultureInfo.InvariantCulture).AddDays(1);
                q = q.Where(x => x.CreatedAt < end);
            }
            var model = await PagingList<Logging>.CreateAsync(q.OrderByDescending(x => x.CreatedAt), PageSize, pageindex);
            model.RouteValue = new RouteValueDictionary
            {
                {"txtSearch", txtSearch},
                {"userId", userId},
                {"type", type},
                {"startTime", startTime},
                {"endTime", endTime}
            };
            var modelCollection = new ModelCollection();
            modelCollection.AddModel("ListData", model);
            modelCollection.AddModel("Page", (pageindex - 1) * PageSize + 1);
            modelCollection.AddModel("ListUser", _iApplicationUserRepository.FindAll().ToList());
            return View(modelCollection);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public JsonResult Delete(int id)
        {
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Admin@LoggingsController@Delete")]
        [NonLoad]
        public JsonResult DeleteAll(List<int> id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu lịch sử hoạt động");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                int rs = this._iLoggingRepository.DeleteAll(id);
                ToastMessage(1, $"Xóa thành công {rs} bản ghi");
                this._iLogger.LogInformation($"Xóa thành công {rs} lịch sử hoạt động");
                return Json(new
                {
                    msg = "successful",
                    content = ""
                });
            }
            catch (Exception ex)
            {
                ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                this._iLogger.LogError(ex,$"Xóa dữ liệu lỗi, liên hệ người quản trị: id {id}");
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                });
            }
        }
    }
}