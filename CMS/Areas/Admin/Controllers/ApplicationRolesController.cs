using CMS.Areas.Admin.ViewModels.ApplicationRole;
using CMS.Controllers;
using CMS_Access.Repositories;
using CMS_EF.Models.Identity;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Obsolete]
    public class ApplicationRolesController : BaseController
    {
        private readonly IApplicationRoleRepository _iApplicationRoleRepository;

        private readonly IClaimUserRepository _iClaimUserRepository;
        private readonly ILogger<ApplicationRolesController> _iLogger;

        public ApplicationRolesController(
            IApplicationRoleRepository iApplicationRoleRepository, ILogger<ApplicationRolesController> iLogger,
             IClaimUserRepository iClaimUserRepository
        )
        {
            this._iApplicationRoleRepository = iApplicationRoleRepository;
            this._iClaimUserRepository = iClaimUserRepository;
            this._iLogger = iLogger;
        }

        [Authorize(Policy = "PermissionMVC")]
        public async Task<IActionResult> Index(string txtSearch, int pageindex = 1)
        {
            var query = _iApplicationRoleRepository.FindAll();
            if (!string.IsNullOrWhiteSpace(txtSearch))
                query = query.Where(p => EF.Functions.Like(p.Name, "%" + txtSearch.Trim() + "%"));
            var model = await PagingList<ApplicationRole>.CreateAsync(query.OrderByDescending(x => x.Id), PageSize, pageindex);
            model.RouteValue = new RouteValueDictionary
            {
                {"txtSearch", txtSearch}
            };
            var rs = new IndexApplicationRoleViewModel
            {
                Page = (pageindex - 1) * PageSize + 1,
                ListData = model,
                Configuration = Configuration
            };
            return View(rs);
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Details(int? id)
        {
            if (id == null) return NotFound();
            var applicationRole = _iApplicationRoleRepository.FindById((int)id);
            if (applicationRole == null) return NotFound();
            var listApplicationRole = _iApplicationRoleRepository.GetControllerActionByRole(applicationRole.Id);
            var rs = new DetailApplicationRoleViewModel
            {
                Role = applicationRole,
                ListRoleControllerAction = listApplicationRole.Select(x => new ExtendRoleController()
                                                                                       {
                                                                                           Id = x.Id,
                                                                                           Name = x.Name,
                                                                                           Title = x.Title,
                                                                                           ListAction = x.ListAction.Select(a => new ExtendRoleAction()
                                                                                           {
                                                                                               Id = a.Id,
                                                                                               Name = a.Name,
                                                                                               Title = a.Title,
                                                                                               ControllerId = a.ControllerId,
                                                                                               IsChecked = a.IsChecked
                                                                                           }).ToList()
                                                                                       }).ToList(),
            };
            return View(rs);
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Create()
        {
            var listApplicationRole = _iApplicationRoleRepository.GetControllerActionByRole(0);
            var rs = new CreateApplicationRoleViewModel
            {
                ListRoleControllerAction = listApplicationRole.Select(x => new ExtendRoleController()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Title = x.Title,
                    ListAction = x.ListAction.Select(a => new ExtendRoleAction()
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Title = a.Title,
                        ControllerId = a.ControllerId,
                        IsChecked = a.IsChecked
                    }).ToList()
                }).ToList()
            };
            return View(rs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Create(CreateApplicationRoleViewModel createApplicationRoleView)
        {
            if (ModelState.IsValid)
                try
                {
                    ApplicationRole role = new ApplicationRole
                    {
                        Description = createApplicationRoleView.Description,
                        Name = createApplicationRoleView.Name
                    };
                    var result = _iApplicationRoleRepository.InsertRoleByUser(
                        role,
                        createApplicationRoleView.ListRoleControllerAction.Select(x => new RoleController()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Title = x.Title,
                            ListAction = x.ListAction.Select(a => new RoleAction()
                            {
                                Id = a.Id,
                                Name = a.Name,
                                Title = a.Title,
                                ControllerId = a.ControllerId,
                                IsChecked = a.IsChecked
                            }).ToList()
                        }).ToList()
                    );

                    if (result > 0)
                    {
                        ReloadClaimRole();
                        ToastMessage(1, "Thêm mới dữ liệu thành công");
                        return RedirectToAction(nameof(Details), new { id = result });
                    }

                    ToastMessage(-1, "Lỗi không thể thêm mới, liên hệ người quản trị");
                }
                catch (Exception)
                {
                    ToastMessage(-1, "Lỗi không thể thêm mới, liên hệ người quản trị");
                    return View(createApplicationRoleView);
                }

            return View(createApplicationRoleView);
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var applicationRole = _iApplicationRoleRepository.FindById((int)id);
            if (applicationRole == null) return NotFound();
            var listApplicationRole = _iApplicationRoleRepository.GetControllerActionByRole(applicationRole.Id);
            var rs = new EditApplicationRoleViewModel
            {
                Id = applicationRole.Id,
                Name = applicationRole.Name,
                Description = applicationRole.Description,
                ListRoleControllerAction = listApplicationRole.Select(x => new ExtendRoleController()
                {
                    Id = x.Id,
                    Name = x.Name,
                    Title = x.Title,
                    ListAction = x.ListAction.Select(a => new ExtendRoleAction()
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Title = a.Title,
                        ControllerId = a.ControllerId,
                        IsChecked = a.IsChecked
                    }).ToList()
                }).ToList()
            };
            return View(rs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Edit(int id, EditApplicationRoleViewModel editApplicationRoleView)
        {
            if (id != editApplicationRoleView.Id) return NotFound();

            if (ModelState.IsValid)
                try
                {
                    ApplicationRole role = new ApplicationRole
                    {
                        Description = editApplicationRoleView.Description,
                        Name = editApplicationRoleView.Name
                    };
                    var result = _iApplicationRoleRepository.UpdateRoleByUser(
                        id,
                        role,
                        editApplicationRoleView.ListRoleControllerAction.Select(x => new RoleController()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Title = x.Title,
                            ListAction = x.ListAction.Select(a => new RoleAction()
                            {
                                Id = a.Id,
                                Name = a.Name,
                                Title = a.Title,
                                ControllerId = a.ControllerId,
                                IsChecked = a.IsChecked
                            }).ToList()
                        }).ToList()
                    );
                    if (result == 1)
                    {
                        ReloadClaimRole();
                        ToastMessage(1, "Chỉnh sửa nhóm quyền thành công");
                        return RedirectToAction(nameof(Details), new { id });
                    }

                    ToastMessage(-1, "Chỉnh sửa nhóm quyền lỗi, liên hệ người quản trị");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationRoleExists(id)) return NotFound();
                    ToastMessage(-1, "Chỉnh sửa nhóm quyền lỗi, liên hệ người quản trị");
                }

            return View(editApplicationRoleView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu");
                return Json(new
                {
                    msg = "fail",
                    content = "Nhóm quyền này không tồn tại trong hệ thống, không thể xóa"
                });
            }


            try
            {
                var result = _iApplicationRoleRepository.DeleteApplicationRole((int)id);
                if (result)
                {
                    ReloadClaimRole();
                    ToastMessage(1, "Xóa nhóm quyền thành công");
                    return Json(new
                    {
                        msg = "successful",
                        content = "Xóa nhóm quyền thành công"
                    });
                }
                else
                {
                    ToastMessage(-1, "Xóa nhóm quyền lỗi");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Không thể xóa nhóm quyền này, vui lòng liên hệ với người quản trị"
                    });
                }
            }
            catch (Exception)
            {
                ToastMessage(-1, "Xóa nhóm quyền lỗi");
                return Json(new
                {
                    msg = "fail",
                    content = "Không thể xóa nhóm quyền này, vui lòng liên hệ với người quản trị"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Admin@ApplicationRolesController@Delete")]
        [NonLoad]
        public JsonResult DeleteAll(List<int> id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu nhóm quyền");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                int rs = this._iApplicationRoleRepository.DeleteAll(id);
                if (rs >= 0)
                {
                    ReloadClaimRole();
                    ToastMessage(1, $"Xóa thành công {rs} bản ghi");
                    this._iLogger.LogInformation($"Xóa thành công {rs} nhóm quyền");
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

        private bool ApplicationRoleExists(int id)
        {
            return _iApplicationRoleRepository.IsCheckById(id);
        }

        #region private reload claim

        private void ReloadClaimRole()
        {
            try
            {
                var user = User as ClaimsPrincipal;
                var identity = user.Identity as ClaimsIdentity;
                var controllerActionType = Configuration.GetSection(CmsClaimType.ClaimType).GetValue<string>(CmsClaimType.ControllerAction);
                var listControllerAction = _iClaimUserRepository.GetControllerActionRoleByUserClaimType(UserInfo.UserId, controllerActionType);
                identity?.FindAll(controllerActionType).ToList().ForEach(x =>
                {
                    identity.TryRemoveClaim(x);
                });
                listControllerAction?.ForEach(x =>
                {
                    identity?.AddClaim(new Claim(controllerActionType, x.ToUpper()));
                });
            }
            catch (Exception ex)
            {
                this._iLogger.LogError("Reload claim", ex.Message);
            }

        }

        #endregion
    }
}