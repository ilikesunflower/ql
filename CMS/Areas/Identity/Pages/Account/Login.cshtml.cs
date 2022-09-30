using CMS_EF.Models.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CMS.Extensions.Validate;
using CMS_Lib.Util;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CMS.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _iHttpContextAccessor;

        public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger,
            IConfiguration configuration,IHttpContextAccessor iHttpContextAccessor)
        {
            _signInManager = signInManager;
            _logger = logger;
            this._configuration = configuration;
            _iHttpContextAccessor = iHttpContextAccessor;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
            [ValidXss]
            public string UserName { get; set; }

            [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
            [DataType(DataType.Password)]
            [ValidXss]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl ??= Url.Content("~/");
            this._iHttpContextAccessor.HttpContext?.Session.Clear();

            // Clear the existing external cookie to ensure a clean login process
            if (HttpContext.User.Identity!.IsAuthenticated)
            {
                if (HttpContext.User.Identities.Any(i => i.AuthenticationType ==CmsConsts.WsFederationAuth))
                {
                    await _signInManager.SignOutAsync();
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignOutAsync(CmsConsts.WsFederationAuth);
                }else
                {
                    await _signInManager.SignOutAsync();
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                }   
            }

            ReturnUrl = returnUrl;
        }

        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                try
                {
                    SignInResult result = null;
                    result = await _signInManager.PasswordSignInAsync(Input.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: true);
                    if (result.Succeeded)
                    {
                        var user = _signInManager.UserManager.Users.FirstOrDefault(x => x.UserName == Input.UserName && x.Flag == 0);
                        if (user != null && user.IsActive != 1)
                        {
                            await this._signInManager.SignOutAsync();
                            _logger.LogWarning($"Tài khoản {Input.UserName} chưa được kích hoạt");
                            ModelState.AddModelError("Input.Password", "Tài khoản chưa được kích hoạt");
                            return Page();
                        }
                        _logger.LogInformation($"Tài khoản {user.UserName} đăng nhập thành công.");
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, _signInManager.CreateUserPrincipalAsync(user).Result, new AuthenticationProperties { IsPersistent = true });
                        return LocalRedirect(returnUrl);
                    }
                    if (result.RequiresTwoFactor)
                    {
                        _logger.LogWarning($"Tài khoản {Input.UserName} chuyển đến trang xác thực OTP.");
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
                    }
                    if (result.IsLockedOut)
                    {
                        _logger.LogWarning($"Tài khoản {Input.UserName} đã bị khóa, vui lòng quay lại sau {_configuration.GetSection("AppSetting").GetValue<int>("DefaultLockoutTimeSpan")} Phút");
                        ModelState.AddModelError("Input.Password", "Tài khoản đã bị khóa, vui lòng quay lại sau " + _configuration.GetSection("AppSetting").GetValue<int>("DefaultLockoutTimeSpan") + " Phút");
                        return Page();
                    }
                    else
                    {
                        _logger.LogWarning($"{Input.UserName} Tài khoản hoặc mật khẩu không đúng.");
                        ModelState.AddModelError("Input.Password", "Tài khoản hoặc mật khẩu không đúng.");
                        return Page();
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex,$"{Input.UserName} Tài khoản hoặc mật khẩu không đúng.");
                    ModelState.AddModelError("Input.Password", "Tài khoản hoặc mật khẩu không đúng.");
                    return Page();
                }
            }
            return Page();
        }
    }
}
