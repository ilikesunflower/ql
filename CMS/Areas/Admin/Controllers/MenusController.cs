using CMS.Areas.Admin.ViewModels.Menus;
using CMS.Controllers;
using CMS.Models.ModelContainner;
using CMS_Access.Repositories;
using CMS_EF.Models;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Obsolete]
    public class MenusController : BaseController
    {
        private readonly IMenuRepository _iMenuRepository;
        private readonly IApplicationActionRepository _iApplicationActionRepository;
        private readonly IApplicationControllerRepository _iApplicationControllerRepository;
        private readonly ILogger _iLogger;

        public MenusController(ILogger<MenusController> iLogger, IMenuRepository iMenuRepository,
            IApplicationControllerRepository iApplicationControllerRepository,
            IApplicationActionRepository iApplicationActionRepository)
        {
            _iMenuRepository = iMenuRepository;
            _iApplicationActionRepository = iApplicationActionRepository;
            _iApplicationControllerRepository = iApplicationControllerRepository;
            _iLogger = iLogger;
        }

        // GET: Admin/Menus
        [Authorize(Policy = "PermissionMVC")]
        public Task<IActionResult> Index(string txtSearch, int? pid)
        {
            var query = _iMenuRepository.FindAll();
            if (!string.IsNullOrEmpty(txtSearch))
            {
                query = query.Where(x => x.Name.Contains(txtSearch.Trim()));
            }
            if (pid.HasValue)
            {
                var pData = _iMenuRepository.FindById(pid.Value);
                if (pData != null)
                {
                    query = query.Where(x =>  x.Rgt.StartsWith(pData.Rgt) );
                }
            }
            List<Menu> listData = query.OrderBy(x => x.Lft).ToList();
         
            ModelCollection rs = new ModelCollection();
            rs.AddModel("txtSearch", txtSearch);
            rs.AddModel("pid", pid);
            rs.AddModel("ListData", listData);
            rs.AddModel("ListMenu", _iMenuRepository.GetDisplayMenu());
            ILoggingService.Infor(_iLogger, "Xem danh sách menu");
            return Task.FromResult<IActionResult>(View(rs));
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = _iMenuRepository.FindById((int)id);
            if (menu == null)
            {
                return NotFound();
            }

            var menuParent = _iMenuRepository.FindById(menu.Pid);
            ILoggingService.Infor(_iLogger, "Xem chi tiết menu");
            ModelCollection rs = new ModelCollection();
            rs.AddModel("Menu", menu);
            rs.AddModel("MenuParent", menuParent);
            rs.AddModel("ControllerAction", _iApplicationActionRepository.FindById(menu.ActionId ?? 0));
            return View(rs);
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Create()
        {
            CreateViewModel rs = new CreateViewModel
            {
                ListMenus = _iMenuRepository.GetDisplayMenu(),
                ListControllers = _iApplicationControllerRepository.GetAllController().ToList()
            };
            return View(rs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Create(CreateViewModel menu)
        {
            if (ModelState.IsValid)
            {
                Menu insertMenu = new Menu
                {
                    Name = menu.Name,
                    Status = menu.Status,
                    Pid = menu.Pid,
                    Url = menu.Url.Trim(),
                    ControllerId = menu.ControllerId ?? 0,
                    ActionId = menu.ActionId ?? 0,
                    CssClass = menu.CssClass
                };
                var rs = _iMenuRepository.InsertMenu(insertMenu);
                if (rs != null)
                {
                    ToastMessage(1, "Thêm mới menu thành công");
                    ILoggingService.Infor(_iLogger, "Thêm mới menu thành công", "id:" + rs.Id);
                    HttpContext.Session.Remove(CmsClaimType.Menu);
                    return RedirectToAction(nameof(Details), new { rs.Id });
                }
                ToastMessage(-1, "Thêm mới menu lỗi, liên hệ người quản trị");
                ILoggingService.Infor(this._iLogger, "Thêm mới menu lỗi");
            }
            menu.ListMenus = _iMenuRepository.GetDisplayMenu();
            menu.ListControllers = _iApplicationControllerRepository.GetAllController().ToList();
            return View(menu);
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var menu = _iMenuRepository.FindById(id.Value);
            if (menu == null)
            {
                return NotFound();
            }

            EditViewModel rs = new EditViewModel
            {
                Id = menu.Id,
                Pid = menu.Pid,
                Name = menu.Name,
                Url = menu.Url,
                CssClass = menu.CssClass,
                Status = menu.Status,
                ControllerId = menu.ControllerId ?? 0,
                ActionId = menu.ActionId ?? 0,
                ListMenus = _iMenuRepository.GetDisplayMenu(),
                ListControllers = _iApplicationControllerRepository.GetAllController().ToList()
            };
            return View(rs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Edit(int id, EditViewModel menu)
        {
            if (id != menu.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Menu updateMenu = new Menu
                    {
                        Id = menu.Id,
                        Name = menu.Name.Trim(),
                        CssClass = menu.CssClass,
                        Pid = menu.Pid,
                        Status = menu.Status,
                        Url = menu.Url.Trim(),
                        ActionId = menu.ActionId ?? 0,
                        ControllerId = menu.ControllerId ?? 0
                    };
                    var rs = _iMenuRepository.UpdateMenu(updateMenu);
                    if (rs)
                    {
                        this.HttpContext.Session.Remove(CmsClaimType.Menu);
                        ToastMessage(1, "Chỉnh sửa menu thành công");
                        ILoggingService.Infor(this._iLogger, "Chỉnh sửa menu thành công", "id:" + id);
                        return RedirectToAction(nameof(Details), new { id });
                    }

                    ToastMessage(-1, "Chỉnh sửa menu lỗi");
                    ILoggingService.Infor(this._iLogger, "Chỉnh sửa menu lỗi", "id:" + id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MenuExists(menu.Id))
                    {
                        return NotFound();
                    }
                }
            }
            menu.ListMenus = _iMenuRepository.GetDisplayMenu();
            menu.ListControllers = _iApplicationControllerRepository.GetAllController().ToList();
            return View(menu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Xóa dữ liệu lỗi, không có dữ liệu");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu về menu, không thể xóa"
                });
            }

            try
            {
                _iMenuRepository.Delete(id.Value);
                this.HttpContext.Session.Remove(CmsClaimType.Menu);
                ILoggingService.Infor(this._iLogger, "Xóa menu thành công");
                ToastMessage(1, "Xóa dữ liệu menu thành công");
                return Json(new
                {
                    msg = "successful",
                    content = "Xóa menu thành công"
                });
            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "Xóa menu lỗi", e.ToString(), e);
                ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu về menu, không thể xóa"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Admin@MenusController@Delete")]
        [NonLoad]
        public JsonResult DeleteAll(List<int> id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu menu");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                int rs = _iMenuRepository.DeleteAll(id);
                this.HttpContext.Session.Remove(CmsClaimType.Menu);
                ToastMessage(1, $"Xóa thành công {rs} bản ghi");
                this._iLogger.LogInformation($"Xóa thành công {rs} menu");
                return Json(new
                {
                    msg = "successful",
                    content = ""
                });
            }
            catch (Exception e)
            {
                ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                this._iLogger.LogError(e,$"Xóa dữ liệu lỗi, liên hệ người quản trị: id {id}");
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public JsonResult ChangeStatus(int id, int status)
        {
            try
            {
                _iMenuRepository.ChangeStatus(id, status);
                ILoggingService.Infor(this._iLogger, "Cập nhật trạng thái menu thành công");
                ToastMessage(1, "Cập nhật trạng thái menu thành công");
                HttpContext.Session.Remove(CmsClaimType.Menu);
                return Json(new
                {
                    msg = "successful",
                    content = "Cập nhật trạng thái menu thành công"
                });
            }
            catch (Exception e)
            {
                ILoggingService.Error(this._iLogger, "Cập nhật trạng thái menu lỗi", e.ToString(), e);
                return Json(new
                {
                    msg = "fail",
                    content = "Cập nhật trạng thái menu lỗi: " + e
                });
            }
        }
        
        
        [HttpPost]
        // [ValidateAntiForgeryToken]
        // [Authorize(Policy = "PermissionMVC")]
        public JsonResult UpdateOrder(UpdateOrderViewModel form)
        {
            try
            {
                _iMenuRepository.UpdateOrder(form.Ids,form.Parent);
                ILoggingService.Infor(_iLogger, "Cập nhật trạng thái menu thành công");
                HttpContext.Session.Remove(CmsClaimType.Menu);
                ToastMessage(1,"Cập nhật vị trí menu thành công");
                return Json(new
                {
                    msg = "successful",
                    content = "Cập nhật vị trí menu thành công"
                });
            }
            catch (Exception e)
            {
                ILoggingService.Error(_iLogger, "Cập nhật trạng thái menu lỗi", e.ToString(), e);
                return Json(new
                {
                    msg = "fail",
                    content = "Cập nhật vị trí menu lỗi: " + e
                });
            }
        }


        private bool MenuExists(int id)
        {
            return _iMenuRepository.IsCheckById(id);
        }
    }
}
