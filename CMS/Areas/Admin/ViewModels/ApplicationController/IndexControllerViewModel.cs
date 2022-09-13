using Microsoft.Extensions.Configuration;

namespace CMS.Areas.Admin.ViewModels.ApplicationController
{
    public class IndexControllerViewModel
    {
        public int Page { set; get; }

        public ReflectionIT.Mvc.Paging.PagingList<CMS_EF.Models.Identity.ApplicationController> ListData { set; get; }

        public IConfiguration Configuration { set; get; }
    }
}
