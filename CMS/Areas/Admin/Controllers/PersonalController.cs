using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using CMS.Areas.Admin.ViewModels.Personal;
using CMS.Controllers;
using CMS.Extensions.StateTempData;
using CMS.Services.Claims;
using CMS_Access.Repositories;
using CMS_EF.Models.Identity;
using CMS_Lib.Extensions.Attribute;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Admin.Controllers
{
    [Area("Admin")]
    [NonLoad]
    public class PersonalController : BaseController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UrlEncoder _urlEncoder;
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
        private readonly IApplicationUserRepository _iApplicationUserRepository;
        private readonly ILogger _iLogger;
        private readonly IClaimService _iClaimService;

        public string SharedKey { get; set; }
        public string AuthenticatorUri { get; set; }

        public PersonalController(ILogger<PersonalController> iLogger, UserManager<ApplicationUser> userManager,
            UrlEncoder urlEncoder, SignInManager<ApplicationUser> signInManager, IClaimService iClaimService,
            IApplicationUserRepository iApplicationUserRepository)
        {
            this._userManager = userManager;
            this._urlEncoder = urlEncoder;
            this._signInManager = signInManager;
            this._iClaimService = iClaimService;
            this._iApplicationUserRepository = iApplicationUserRepository;
            this._iLogger = iLogger;
        }

        // GET: Admin/Personal/Details
        // [ClaimRequirement("CONTROLLER@ACTION", "ApplicationUsersController@Index , ApplicationUsersController@Edit ")]
        // [ClaimOrRequirement("CONTROLLER@ACTION", "ApplicationUsersController@Index")]
        [NoActiveMenu]
        [HttpGet]
        public async Task<IActionResult> Details()
        {
            var applicationUser = _iApplicationUserRepository.FindById(UserInfo.UserId);
            if (applicationUser == null)
            {
                return NotFound();
            }

            DetailsViewModel rs = new DetailsViewModel
            {
                Id = applicationUser.Id,
                Description = applicationUser.Description,
                Email = applicationUser.Email,
                FullName = applicationUser.FullName,
                Image = applicationUser.Image,
                // Organization = applicationUser.Organization,
                PhoneNumber = applicationUser.PhoneNumber,
                Sex = applicationUser.Sex,
                UserName = applicationUser.UserName,
                HasAuthenticator = await _userManager.GetTwoFactorEnabledAsync(applicationUser)
            };
            return View(rs);
        }

        [NoActiveMenu]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            ChangePasswordViewModel rs = new ChangePasswordViewModel();
            return View(rs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoActiveMenu]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (User.HasClaim(CmsClaimType.UserType, "1"))
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var checkpass = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash,
                    changePasswordViewModel.Password);
                if (checkpass == PasswordVerificationResult.Success)
                {
                    if (!changePasswordViewModel.Password.Equals(changePasswordViewModel.PasswordNew))
                    {
                        string resetToken = _userManager.GeneratePasswordResetTokenAsync(user).Result;
                        var resultPass = _userManager
                            .ResetPasswordAsync(user, resetToken, changePasswordViewModel.PasswordNew).Result;
                        if (resultPass.Succeeded)
                        {
                            ILoggingService.Infor(this._iLogger, "Đổi mật khẩu thành công ", user.Id.ToString());
                            ToastMessage(1, "Đổi mật khẩu thành công");
                            return Redirect("/Identity/Account/Logout");
                        }
                        else
                        {
                            ILoggingService.Infor(this._iLogger, "Đổi mật khẩu lỗi ", user.Id.ToString());
                            ToastMessage(-1, "Đổi mật khẩu không thành công");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("PasswordNew",
                            "Nhập mật khẩu mới không được trùng với mật khẩu hiện tại");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Nhập mật khẩu hiện tại không đúng");
                }
            }

            return View(changePasswordViewModel);
        }

        [NoActiveMenu]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();
            if (User.HasClaim(CmsClaimType.UserType, "1"))
            {
                return NotFound();
            }
            var applicationUser = _iApplicationUserRepository.FindById((int) id);
            if (applicationUser == null) return NotFound();
            var rs = new EditProfileViewModel
            {
                Id = applicationUser.Id,
                UserName = applicationUser.UserName,
                FullName = applicationUser.FullName,
                Email = applicationUser.Email,
                Sex = applicationUser.Sex,
                Image = applicationUser.Image,
                PhoneNumber = applicationUser.PhoneNumber,
            };
            return View(rs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoActiveMenu]
        public async Task<IActionResult> Edit(int id, EditProfileViewModel editProfileViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var u = _iApplicationUserRepository.FindById(id);
                    if (u == null)
                    {
                        return NotFound();
                    }
                    if (User.HasClaim(CmsClaimType.UserType, "1"))
                    {
                        return NotFound();
                    }
                    ApplicationUser user = new ApplicationUser();
                    user.FullName = editProfileViewModel.FullName;
                    user.Image = editProfileViewModel.Image;
                    user.Sex = editProfileViewModel.Sex;
                    user.Email = editProfileViewModel.Email;
                    user.PhoneNumber = editProfileViewModel.PhoneNumber;
                    var result = _iApplicationUserRepository.UpdateProfileUser(user, id);
                    if (result == 1)
                    {
                        ILoggingService.Infor(this._iLogger, "Cập nhật thông tin cá nhân thành công",
                            user.Id.ToString());
                        ToastMessage(1, "Cập nhật thông tin cá nhân thành công");
                        _iClaimService.ReloadInfoUser(HttpContext.User, u);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, _signInManager.CreateUserPrincipalAsync(u).Result, new AuthenticationProperties() { IsPersistent = true });
                        return RedirectToAction(nameof(Details), new { id });
                    }
                    else
                    {
                        ILoggingService.Infor(this._iLogger, "Cập nhật thông tin cá nhân lỗi ", user.Id.ToString());
                        ToastMessage(-1, "Cập nhật thông tin cá nhân không thành công");
                    }
                }
                catch (Exception)
                {
                    if (!ApplicationUserExists(id)) return NotFound();
                }
            }

            editProfileViewModel.Id = id;
            return View(editProfileViewModel);
        }


        #region QR code

        [ImportModelState]
        [NoActiveMenu]
        [HttpGet]
        public async Task<IActionResult> EnableAuthenticator()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Tài khoản không tồn tại '{_userManager.GetUserId(User)}'.");
            }

            await LoadSharedKeyAndQrCodeUriAsync(user);
            EnableAuthenticatorViewModel rs = new EnableAuthenticatorViewModel {AuthenticatorUri = AuthenticatorUri};
            return View(rs);
        }

        [HttpPost]
        [ExportModelState]
        [ValidateAntiForgeryToken]
        [NoActiveMenu]
        public async Task<IActionResult> EnableAuthenticator(EnableAuthenticatorViewModel enableAuthenticator)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Tài khoản không tồn tại hoặc đã bị khóa'{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadSharedKeyAndQrCodeUriAsync(user);
                enableAuthenticator.AuthenticatorUri = AuthenticatorUri;
                return View(enableAuthenticator);
            }

            // Strip spaces and hypens
            var verificationCode = enableAuthenticator.Code.Replace(" ", string.Empty).Replace("-", string.Empty);

            var is2FaTokenValid = await _userManager.VerifyTwoFactorTokenAsync(
                user, _userManager.Options.Tokens.AuthenticatorTokenProvider, verificationCode);

            if (!is2FaTokenValid)
            {
                ModelState.AddModelError("Code", "Mã xác nhận không đúng.");
                await LoadSharedKeyAndQrCodeUriAsync(user);
                enableAuthenticator.AuthenticatorUri = AuthenticatorUri;
                ILoggingService.Error(this._iLogger, "Cấu hình xác thực otp lỗi ", user.Id.ToString());
                return View(enableAuthenticator);
            }

            await _userManager.SetTwoFactorEnabledAsync(user, true);
            ILoggingService.Infor(this._iLogger, "Cấu hình xác thực otp thành công ", user.Id.ToString());
            ToastMessage(1, "Cấu hình xác thực otp thành công");
            return Redirect("/Identity/Account/Logout");
        }

        private async Task LoadSharedKeyAndQrCodeUriAsync(ApplicationUser user)
        {
            // Load the authenticator key & QR code URI to display on the form
            var unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            if (string.IsNullOrEmpty(unformattedKey))
            {
                await _userManager.ResetAuthenticatorKeyAsync(user);
                unformattedKey = await _userManager.GetAuthenticatorKeyAsync(user);
            }

            SharedKey = FormatKey(unformattedKey);
            AuthenticatorUri = GenerateQrCodeUri(user.UserName, unformattedKey);
        }

        private static string FormatKey(string unformattedKey)
        {
            var result = new StringBuilder();
            int currentPosition = 0;
            while (currentPosition + 4 < unformattedKey.Length)
            {
                result.Append(unformattedKey.Substring(currentPosition, 4)).Append(' ');
                currentPosition += 4;
            }

            if (currentPosition < unformattedKey.Length)
            {
                result.Append(unformattedKey[currentPosition..]);
            }

            return result.ToString().ToLowerInvariant();
        }

        private string GenerateQrCodeUri(string name, string unformattedKey)
        {
            return string.Format(
                AuthenticatorUriFormat,
                this._urlEncoder.Encode(Configuration.GetSection(CmsConsts.WebSetting)
                    .GetValue<string>(CmsConsts.WebName)),
                this._urlEncoder.Encode(name),
                unformattedKey);
        }

        #endregion

        #region disable 2fa

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Disable2Fa()
        {
            var user = await _userManager.GetUserAsync(User);
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

        #region Reset Authenticator

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResetAuthenticator()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    ToastMessage(-1, "Tài khoản không tồn tại hoặc đã bị khóa");
                    return Json(new
                    {
                        msg = "fail",
                        content = "Tài khoản không tồn tại hoặc đã bị khóa"
                    });
                }

                await _userManager.SetTwoFactorEnabledAsync(user, false);
                await _userManager.ResetAuthenticatorKeyAsync(user);
                await _signInManager.RefreshSignInAsync(user);
                ToastMessage(1, "Đặt lại mã xác thực thành công");
                return Json(new
                {
                    msg = "successful",
                    content = "Đặt lại mã xác thực thành công"
                });
            }
            catch
            {
                ToastMessage(-1, "Lỗi không thể đặt lại mã xác thực, vui lòng liên hệ người quản trị");
                return Json(new
                {
                    msg = "fail",
                    content = "Lỗi không thể đặt lại mã xác thực, vui lòng liên hệ người quản trị"
                });
            }
        }

        #endregion

        private bool ApplicationUserExists(int id)
        {
            return _iApplicationUserRepository.IsCheckById(id);
        }
    }
}