using CMS.Extensions.Validate;
using CMS_EF.Models.Identity;
using Microsoft.Extensions.Configuration;
using ReflectionIT.Mvc.Paging;

namespace CMS.Areas.Admin.ViewModels.ApplicationController
{
    public class IndexActionViewModel
    {
        public IConfiguration Configuration { set; get; }
        public int ControllerId { get; set; }
        [ValidXss]
        public string ControllerName { get; set; }
        public PagingList<ApplicationAction> ListAction { get; set; }
    }
}
