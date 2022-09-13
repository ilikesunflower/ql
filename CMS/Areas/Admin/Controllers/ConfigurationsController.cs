using CMS.Areas.Admin.ViewModels.Configuration;
using CMS.Controllers;
using CMS.Models.ModelContainner;
using CMS_Access.Repositories;
using CMS_EF.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Obsolete]
    public class ConfigurationsController : BaseController
    {
        private readonly IConfigurationRepository _iConfigurationRepository;
        private readonly ILogger _iLogger;

        public ConfigurationsController(ILogger<ConfigurationsController> iLogger, IConfigurationRepository iConfigurationRepository)
        {
            this._iConfigurationRepository = iConfigurationRepository;
            this._iLogger = iLogger;
        }

        [Authorize(Policy = "PermissionMVC")]
        public async Task<IActionResult> Index(string txtKeyword, int pageindex = 1)
        {
            var query = _iConfigurationRepository.FindAll();
            if (!string.IsNullOrWhiteSpace(txtKeyword))
            {
                query = query.Where(p => EF.Functions.Like(p.Name, "%" + txtKeyword.Trim() + "%") 
                                         || EF.Functions.Like(p.Val, "%" + txtKeyword.Trim() + "%"));
            }

            var model = await PagingList<Configuration>.CreateAsync(query.OrderByDescending(x => x.LastModifiedAt), PageSize, pageindex);
            model.RouteValue = new RouteValueDictionary
            {
                {"txtKeyword", txtKeyword},
            };
            var modelCollection = new ModelCollection();
            modelCollection.AddModel("ListData", model);
            modelCollection.AddModel("Page", (pageindex - 1) * PageSize + 1);
            return View(modelCollection);
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var configuration = _iConfigurationRepository.FindById(id.Value);
            if (configuration == null)
            {
                return NotFound();
            }

            return View(configuration);
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Create(CreateViewModel configuration)
        {
            if (ModelState.IsValid)
            {
                Configuration con = new Configuration
                {
                    Name = configuration.Name,
                    Detail = configuration.Detail,
                    Val = configuration.Val,
                    CreatedBy = UserInfo.UserId,
                    CreatedAt = DateTime.Now,
                    LastModifiedBy = UserInfo.UserId,
                    LastModifiedAt = DateTime.Now
                };
                var rs = _iConfigurationRepository.Create(con);
                ILoggingService.Infor(this._iLogger, "Thêm mới cấu hình thành công", "id:" + con.Id);
                ToastMessage(1, "Thêm mới cấu hình thành công");
                return RedirectToAction(nameof(Details), new { con.Id });
            }
            return View(configuration);
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var configuration = _iConfigurationRepository.FindById(id.Value);
            if (configuration == null)
            {
                return NotFound();
            }

            EditViewModel rs = new EditViewModel
            {
                Id = configuration.Id,
                Name = configuration.Name,
                Val = configuration.Val,
                Detail = configuration.Detail
            };
            return View(rs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Edit(int id, EditViewModel configuration)
        {
            if (id != configuration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var con = _iConfigurationRepository.FindById(id);
                    con.Name = configuration.Name;
                    con.Detail = configuration.Detail;
                    con.Val = configuration.Val;
                    con.LastModifiedAt = DateTime.Now;
                    con.LastModifiedBy = UserInfo.UserId;
                    _iConfigurationRepository.Update(con);
                    ILoggingService.Infor(this._iLogger,"Chỉnh sửa cấu hình thành công", "id:" + id);
                    ToastMessage(1, "Chỉnh sửa cấu hình thành công");
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (DbUpdateConcurrencyException)
                {
                    ToastMessage(-1, "Lỗi không thể sửa cấu hình này, Vui lòng liên hệ người quản trị");
                }
            }
            return View(configuration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu cấu hình");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                var configuration = _iConfigurationRepository.FindById(id.Value);
                if (configuration != null)
                {
                    _iConfigurationRepository.Delete(configuration);
                    ToastMessage(1, "Xóa cấu hình thành công");
                    return Json(new
                    {
                        msg = "successful",
                        content = "Xóa cấu hình thành công"
                    });
                }
                else
                {
                    ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Không có dữ liệu, không thể xóa"
                    });
                }
            }
            catch (Exception)
            {
                ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Admin@ConfigurationsController@Delete")]
        [NonLoad]
        public JsonResult DeleteAll(List<int> id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu cấu hình");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                int rs = this._iConfigurationRepository.DeleteAll(id);
                ToastMessage(1, $"Xóa thành công {rs} bản ghi");
                this._iLogger.LogInformation($"Xóa thành công {rs} cấu hình");
                return Json(new
                {
                    msg = "successful",
                    content = ""
                });
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

        private bool ConfigurationExists(int id)
        {
            return _iConfigurationRepository.IsCheckById(id);
        }
    }
}
