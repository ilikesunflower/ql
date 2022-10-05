using CMS.Areas.Admin.ViewModels.ApplicationUser;
using CMS.Controllers;
using CMS.Services.Claims;
using CMS_Access.Repositories;
using CMS_EF.Models.Identity;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Extensions.Claim;
using CMS_Lib.Extensions.Json;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReflectionIT.Mvc.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CMS.Areas.Admin.Const;
using CMS.Areas.Admin.Services.User;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace CMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Obsolete]
    public class ApplicationUsersController : BaseController
    {
        private readonly IApplicationRoleRepository _iApplicationRoleRepository;
        private readonly IApplicationUserRepository _iApplicationUserRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ApplicationUsersController> _ilogger;
        private readonly IClaimService _iClaimService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ApplicationUsersController(ILogger<ApplicationUsersController> ilogger, UserManager<ApplicationUser> userManager,
            IApplicationRoleRepository iApplicationRoleRepository, IApplicationUserRepository iApplicationUserRepository, SignInManager<ApplicationUser> signInManager,
             IClaimService iClaimService)
        {
            this._iApplicationRoleRepository = iApplicationRoleRepository;
            this._iApplicationUserRepository = iApplicationUserRepository;
            this._userManager = userManager;
            this._ilogger = ilogger;
            this._iClaimService = iClaimService;
            this._signInManager = signInManager;
        }

        [Authorize(Policy = "PermissionMVC")]
        public async Task<IActionResult> Index(string txtSearch, string status, int? typeUser, int pageindex = 1)
        {
            var query = _iApplicationUserRepository.FindAll();
            if (!string.IsNullOrWhiteSpace(txtSearch))
                query = query.Where(p => EF.Functions.Like(p.UserName, "%" + txtSearch.Trim() + "%"));


            if (!string.IsNullOrEmpty(status))
            {
                int isStatus = Int32.Parse(status.Trim());
                query = query.Where(x => x.IsActive == isStatus);
            }

            if (typeUser.HasValue)
            {
                query = query.Where(x => x.Type == typeUser);
            }


            var model = await PagingList<ApplicationUser>.CreateAsync(query.OrderBy(x => x.Id), PageSize, pageindex);
            model.RouteValue = new RouteValueDictionary
            {
                {"txtSearch", txtSearch},
                {"status", status},
                {"typeUser", typeUser},
            };
            var rs = new IndexUserViewModel
            {
                Page = (pageindex - 1) * PageSize + 1,
                ListData = model,
                Configuration = Configuration,
            };
            return View(rs);
        }

        [Authorize(Policy = "PermissionMVC")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var applicationUser = _iApplicationUserRepository.FindById((int)id);
            if (applicationUser == null) return NotFound();
            var rs = new DetailUserViewModel
            {
                Id = id.Value,
                UserName = applicationUser.UserName,
                FullName = applicationUser.FullName,
                Address = applicationUser.Address,
                Email = applicationUser.Email,
                Sex = applicationUser.Sex,
                Image = applicationUser.Image,
                PhoneNumber = applicationUser.PhoneNumber,
                IsActive = applicationUser.IsActive,
                ListRoles = _iApplicationRoleRepository.GetAllRoleJoinUseRoles(applicationUser.Id).ToList(),
                HasAuthenticator = await _userManager.GetTwoFactorEnabledAsync(applicationUser),
                TypeLabel = UserConst.GetTypeUser(applicationUser.Type)
            };
            return View(rs);
        }

        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Create()
        {
            var rs = new CreatedUserViewModel
            {
                ListRoles = _iApplicationRoleRepository.GetAllRoleJoinUseRoles(0).ToList(),
            };
            return View(rs);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Create(CreatedUserViewModel createdUserViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = new ApplicationUser
                    {
                        UserName = createdUserViewModel.UserName,
                        Email = createdUserViewModel.Email,
                        Address = createdUserViewModel.Address,
                        FullName = createdUserViewModel.FullName,
                        Image = CmsFunction.IsValidImage(createdUserViewModel.Image) ? "" : createdUserViewModel.Image,
                        Sex = createdUserViewModel.Sex,
                        IsActive = createdUserViewModel.IsActive,
                        PasswordHash = createdUserViewModel.Password,
                        PhoneNumber = createdUserViewModel.PhoneNumber,
                        Type = 0
                    };
                    var result = _iApplicationUserRepository.InsertApplicationUser(user, createdUserViewModel.ListRoles);
                    if (result > 0)
                    {
                        ToastMessage(1, "Thêm mới tài khoản thành công");
                        ILoggingService.Infor(this._ilogger, "Thêm mới tài khoản thành công");
                        return RedirectToAction(nameof(Details), new { id = result });
                    }
                    else
                    {
                        ToastMessage(-1, "Thêm mới tài khoản lỗi, liên hệ người quản trị");
                        ILoggingService.Error(this._ilogger, "Thêm mới tài khoản lỗi");
                    }
                }
                catch (Exception ex)
                {
                    ToastMessage(-1, "Thêm mới tài khoản lỗi, liên hệ người quản trị");
                    ILoggingService.Error(this._ilogger, "Thêm mới tài khoản lỗi",ex.Message,ex);
                }
            }

            createdUserViewModel.Image =
                CmsFunction.IsValidImage(createdUserViewModel.Image) ? "" : createdUserViewModel.Image;
            return View(createdUserViewModel);
        }

        
        [Authorize(Policy = "PermissionMVC")]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            var applicationUser = _iApplicationUserRepository.FindById((int)id);
            if (applicationUser == null) return NotFound();
            var rs = new EditUserViewModel
            {
                Id = applicationUser.Id,
                UserName = applicationUser.UserName,
                FullName = applicationUser.FullName,
                Address = applicationUser.Address,
                Email = applicationUser.Email,
                Sex = applicationUser.Sex,
                Image = applicationUser.Image,
                PhoneNumber = applicationUser.PhoneNumber,
                IsActive = applicationUser.IsActive,
                TypeUser = applicationUser.Type,
                ListRoles = _iApplicationRoleRepository.GetAllRoleJoinUseRoles(applicationUser.Id).ToList(),
            };
            return View(rs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public async Task<IActionResult> Edit(int id, EditUserViewModel editUserViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser user = new ApplicationUser();
                    user.Address = editUserViewModel.Address;
                    user.FullName = editUserViewModel.FullName;
                    user.Image = CmsFunction.IsValidImage(editUserViewModel.Image) ? user.Image : editUserViewModel.Image;
                    user.Sex = editUserViewModel.Sex;
                    user.PasswordHash = editUserViewModel.Password;
                    user.Email = editUserViewModel.Email;
                    user.PhoneNumber = editUserViewModel.PhoneNumber;
                    user.IsActive = editUserViewModel.IsActive;
                    var result = _iApplicationUserRepository.UpdateApplicationUser(user, editUserViewModel.ListRoles, id);
                    if (result == 1)
                    {
                        ToastMessage(1, "Cập nhật thông tin tài khoản thành công");
                        ILoggingService.Infor(this._ilogger, "Chỉnh sửa tài khoản thành công");
                        if (id == UserInfo.UserId)
                        {
                            var u = await _userManager.GetUserAsync(HttpContext.User);
                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, _signInManager.CreateUserPrincipalAsync(u).Result, new AuthenticationProperties() { IsPersistent = true });
                        }
                        else
                        {
                            var u = await _userManager.FindByIdAsync(id.ToString());
                            var isLogin = await _signInManager.CanSignInAsync(u);
                            if (isLogin)
                            {
                                await _userManager.UpdateSecurityStampAsync(u);
                            }
                        }
                        return RedirectToAction(nameof(Details), new { id });
                    }

                    if (result == -2)
                    {
                        ToastMessage(-1, "Cập nhật mật khẩu lỗi");
                        ILoggingService.Infor(this._ilogger, "Cập nhật mật khẩu lỗi", "Tài khoản:" + user.UserName);
                    }
                    else if (result == -3)
                    {
                        ToastMessage(-1, "Mật khẩu này đang được tài khoản này sử dụng, vui lòng nhập mật khẩu khác");
                        ILoggingService.Infor(this._ilogger, "Mật khẩu này đang được tài khoản này sử dụng", "Tài khoản:" + user.UserName);
                    }
                    else
                    {
                        ToastMessage(-1, "Lỗi không thể cập nhật thông tin tài khoản, liên hệ người quản trị");
                        ILoggingService.Infor(this._ilogger, "Cập nhật mật khẩu lỗi", "Tài khoản:" + user.UserName);
                    }
                }
                catch (Exception)
                {
                    if (!ApplicationUserExists(id)) return NotFound();
                }
            }

            editUserViewModel.Id = id;
            editUserViewModel.Image = CmsFunction.IsValidImage(editUserViewModel.Image) ? "" : editUserViewModel.Image;
            return View(editUserViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "PermissionMVC")]
        public JsonResult Delete(int? id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu tài khoản");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu về tài khoản, không thể xóa"
                });
            }

            var rs = _iApplicationUserRepository.DeleteApplicationUser((int)id);
            if (rs)
            {
                ILoggingService.Infor(this._ilogger, "Xóa tài khoản thành công:", "id:" + id);
                ToastMessage(1, "Xóa tài khoản thành công");
                return Json(new
                {
                    msg = "successful",
                    content = "Xóa tài khoản thành công"
                });
            }
            else
            {
                ILoggingService.Error(this._ilogger, "Xóa tài khoản lỗi:", "id:" + id);
                ToastMessage(-1, "Xóa tài khoản lỗi");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu về tài khoản, không thể xóa"
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ClaimRequirement(CmsClaimType.AreaControllerAction, "Admin@ApplicationUsersController@Delete")]
        [NonLoad]
        public JsonResult DeleteAll(List<int> id)
        {
            if (id == null)
            {
                ToastMessage(-1, "Không có dữ liệu tài khoản");
                return Json(new
                {
                    msg = "fail",
                    content = "Không có dữ liệu, không thể xóa"
                });
            }

            try
            {
                int rs = this._iApplicationUserRepository.DeleteAll(id);
                if (rs >= 0)
                {
                    ToastMessage(1, $"Xóa thành công {rs} bản ghi");
                    this._ilogger.LogInformation($"Xóa thành công {rs} tài khoản");
                    return Json(new
                    {
                        msg = "successful",
                        content = ""
                    });
                }
                else
                {
                    ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                    this._ilogger.LogError($"Xóa dữ liệu lỗi, liên hệ người quản trị: id {id}");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                    });
                }
            }
            catch (Exception ex)
            {
                ToastMessage(-1, "Xóa dữ liệu lỗi, liên hệ người quản trị");
                // this._ilogger.LogError($"Xóa dữ liệu lỗi, liên hệ người quản trị: id {id}");
                ILoggingService.Error(this._ilogger, $"Xóa tài khoản lỗi", $"id {id} - ex: {ex.Message}",ex);

                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể xóa bản ghi này, vui lòng nhập liên hệ người quản trị"
                });
            }
        }

        #region disable 2fa

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Disable2Fa(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                ToastMessage(-1, "Tài khoản không tồn tại hoặc đã bị khóa");
                return Json(new
                {
                    msg = "fail",
                    content = "Tài khoản không tồn tại hoặc đã bị khóa"
                });
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                ToastMessage(-1, "Tài khoản chưa kích hoạt tích năng xác thực, không thể tắt tính năng xác thực được");
                return Json(new
                {
                    msg = "fail",
                    content = "Tài khoản chưa kích hoạt tích năng xác thực, không thể tắt tính năng xác thực được"
                });
            }

            var disable2FaResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2FaResult.Succeeded)
            {
                ToastMessage(-1, "Lỗi không thể tắt tính năng xác thực, liên hệ người quản trị");
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể tắt tính năng xác thực, liên hệ người quản trị"
                });
            }
            ToastMessage(1, "Tắt tính năng xác thực thành công");
            return Json(new
            {
                msg = "successful",
                content = "Tắt tính năng xác thực thành công"
            });
        }
        #endregion

        private bool ApplicationUserExists(int id)
        {
            return _iApplicationUserRepository.IsCheckById(id);
        }


    }
}