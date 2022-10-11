using System;
using System.Threading.Tasks;
using CMS_EF.Models.Identity;
using CMS.Extensions.Validate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace CMS.Areas.Identity.Pages.Account
{
    [Authorize]
    [ValidHeader]
    public class LogoutModel : PageModel
    {
        private readonly ILogger<LogoutModel> _logger;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public LogoutModel(ILogger<LogoutModel> logger, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }


        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            try
            {
                HttpContext.Session.Clear();
                this._logger.LogInformation($"Tài khoản {HttpContext.User.Identity?.Name} đăng xuất thành công");
                await _signInManager.SignOutAsync();
                // if (User.Identity!.IsAuthenticated)
                // {
                //     this._logger.LogInformation($"Tài khoản {HttpContext.User.Identity?.Name} đăng xuất thành công");
                //     return SignOut(new AuthenticationProperties() { RedirectUri = "/Identity/Account/Login" },
                //         new[] {CookieAuthenticationDefaults.AuthenticationScheme, IdentityConstants.ExternalScheme,IdentityConstants.TwoFactorUserIdScheme,
                //             IdentityConstants.ApplicationScheme });   
                // }
            }
            catch (Exception)
            {
                // ignored
            }

            return Redirect("/Identity/Account/Login");
        }
    }
}