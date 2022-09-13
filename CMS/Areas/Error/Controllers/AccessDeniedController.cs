using CMS.Controllers;
using CMS_Lib.Extensions.Attribute;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Areas.Error.Controllers
{
    [Area("Error")]
    [NonLoad]
    public class AccessDeniedController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}