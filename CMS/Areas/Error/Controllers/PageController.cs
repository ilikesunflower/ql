using CMS_Lib.Extensions.Attribute;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Areas.Error.Controllers
{
    [Area("Error")]
    [Authorize]
    [NonLoad]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class PageController : Controller
    {

        [Route("/Error/Page/{id}")]
        public ActionResult Index(int id)
        {
            var statusCodeData = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();
            ViewBag.Id = id;

            var exceptionCode = HttpContext.Session.GetString("exception_code");
            if (exceptionCode != null)
            {
                ViewBag.ErrorMessage = exceptionCode;
            }
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            if (exceptionHandlerPathFeature != null)
            {
                ViewBag.ErrorMessage = exceptionHandlerPathFeature.Error.Message;
            }
            switch (id)
            {
                case 404:
                    // ViewBag.ErrorMessage = "Sorry the page you requested could not be found";
                    ViewBag.RouteOfException = statusCodeData.OriginalPath;
                    break;
                case 500:
                    // ViewBag.ErrorMessage = "Sorry something went wrong on the server";
                    ViewBag.RouteOfException = statusCodeData.OriginalPath;
                    break;
            }
            return View();
        }

        [Route("/Error/PageNotFound")]
        public IActionResult PageNotFound()
        {
            return View();
        }
    }
}