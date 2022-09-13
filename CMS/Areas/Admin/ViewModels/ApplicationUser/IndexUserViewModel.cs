using Microsoft.Extensions.Configuration;

namespace CMS.Areas.Admin.ViewModels.ApplicationUser
{
    public class IndexUserViewModel
    {
        public int Page { set; get; }

        public ReflectionIT.Mvc.Paging.PagingList<CMS_EF.Models.Identity.ApplicationUser> ListData { set; get; }

        public IConfiguration Configuration { set; get; }

    }
}
