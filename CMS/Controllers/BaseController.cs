using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using CMS.Models;
using CMS.Services.Loggings;
using CMS_Access.Repositories;
using CMS_Lib.Extensions.Json;
using CMS_Lib.Util;
using CMS.Extensions.Validate;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CMS.Controllers
{
    [Authorize]
    [ValidHeader]
    // [AutoValidateAntiforgeryToken]
    [ValidateAntiForgeryToken]
    [IgnoreAntiforgeryToken]
    public abstract class BaseController : Controller
    {
        protected int PageSize { get; set; }
        protected UserInfo UserInfo { get; set; }
        // ReSharper disable once InconsistentNaming
        protected ILoggingService ILoggingService;
        protected IConfiguration Configuration;
        protected IConfigurationSection AppSetting;
        // ReSharper disable once InconsistentNaming
        protected IHtmlSanitizer IHtmlSanitizer;


        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
            ActiveMenu(context);
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
            LoadSetting(context);
            LoadUser(context);
            DefaultPermission(context);
            base.OnActionExecuting(context);
            if (!CheckUserActive(context))
            {
                var msg = WebUtility.UrlEncode("Tài khoản chưa được kích hoạt");
                context.HttpContext.Response.Redirect($"/Identity/Account/Logout?msg="+msg);
            }
        }
        

        private bool CheckUserActive(ActionExecutingContext context)
        {
            int isActive = Int32.Parse(context.HttpContext.User.FindFirstValue(CmsClaimType.IsActiveUser) ?? "0");
            if (isActive == 1)
            {
                return true;
            }
            return false;
        }
        
        private void LoadUser(ActionExecutingContext context)
        {
            
            UserInfo = new UserInfo()
            {
                UserId = Int32.Parse(context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0"),
                UserName = context.HttpContext.User.FindFirstValue(CmsClaimType.UserName) ?? ""
            };
        }

        private void LoadSetting(ActionExecutingContext context)
        {
            ILoggingService = context.HttpContext.RequestServices.GetRequiredService<ILoggingService>();
            Configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            IHtmlSanitizer = context.HttpContext.RequestServices.GetRequiredService<IHtmlSanitizer>();
            AppSetting = Configuration.GetSection(CmsConsts.AppSetting);
            PageSize = AppSetting.GetValue<int>(CmsConsts.PageSize);
        }

        private void ResetPermission()
        {
            TempData.Remove("isCreate");
            TempData.Remove("isImport");
            TempData.Remove("isView");
            TempData.Remove("isEdit");
            TempData.Remove("isDelete");
        }

        private void DefaultPermission(ActionExecutingContext context)
        {
            var controller = context.RouteData.Values["controller"]?.ToString();
            var area = context.RouteData.Values["area"]?.ToString();
                ResetPermission();
                string areaController = $"{area?.ToUpper()}@{controller?.ToUpper() + "CONTROLLER"}";
                TempData["isCreate"] = context.HttpContext.User.HasClaim(CmsClaimType.AreaControllerAction, areaController + "@" + "Create".ToUpper());
                TempData["isImport"] = context.HttpContext.User.HasClaim(CmsClaimType.AreaControllerAction, areaController + "@" + "Create".ToUpper());
                TempData["isView"] = context.HttpContext.User.HasClaim(CmsClaimType.AreaControllerAction, areaController + "@" + "Details".ToUpper());
                TempData["isEdit"] = context.HttpContext.User.HasClaim(CmsClaimType.AreaControllerAction, areaController + "@" + "Edit".ToUpper());
                TempData["isDelete"] = context.HttpContext.User.HasClaim(CmsClaimType.AreaControllerAction, areaController + "@" + "Delete".ToUpper());
                if (Configuration.GetSection("WebSetting:AllPermission").Get<int>() == 1)
                {
                    TempData["isCreate"] = true;
                    TempData["isImport"] = true;
                    TempData["isView"] = true;
                    TempData["isEdit"] = true;
                    TempData["isDelete"] = true;
                }
        }

        protected void ToastMessage(int? type, string message)
        {
            TempData[ResultMessage.IsShowMessage] = type;
            TempData[ResultMessage.ContentMessage] = message;
        }

        private void ActiveMenu(ActionExecutedContext context)
        {
            try
            {
                var listMenu = JsonService.DeserializeObject<List<MenuNav>>(context.HttpContext.Session.GetString(CmsClaimType.Menu)!);
                if (listMenu is {Count: > 0})
                {
                    string url = $"/{context.HttpContext.GetRouteData().Values["area"]}/{context.HttpContext.GetRouteData().Values["controller"]}/{context.HttpContext.GetRouteData().Values["action"]}".ToLower();
                    string action = context.HttpContext.GetRouteData().Values["action"]?.ToString();
                    if (action != null && action.ToLower() == "index" )
                    {
                        string urlIndex = $"/{context.HttpContext.GetRouteData().Values["area"]}/{context.HttpContext.GetRouteData().Values["controller"]}".ToLower();
                        if (listMenu.Any(x => x.Url != string.Empty && x.Url?.ToLower().Trim() == urlIndex))
                        {
                            context.HttpContext.Session.SetString(CmsClaimType.MenuActive, urlIndex);
                            return;
                        }
                    }
                    if (listMenu.Any(x => x.Url != string.Empty && x.Url?.ToLower().Trim() == url))
                    {
                        context.HttpContext.Session.SetString(CmsClaimType.MenuActive, url);
                    }

                    string r = context.HttpContext.Request.Path;
                    if (r == "/")
                    {
                        context.HttpContext.Session.SetString(CmsClaimType.MenuActive, "/");
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
