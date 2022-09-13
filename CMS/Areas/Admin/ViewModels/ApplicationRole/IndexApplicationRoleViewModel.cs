using Microsoft.Extensions.Configuration;

namespace CMS.Areas.Admin.ViewModels.ApplicationRole
{
    public class IndexApplicationRoleViewModel
    {
        public int Page { set; get; }

        public ReflectionIT.Mvc.Paging.PagingList<CMS_EF.Models.Identity.ApplicationRole> ListData { set; get; }

        public IConfiguration Configuration { set; get; }
    }
}
