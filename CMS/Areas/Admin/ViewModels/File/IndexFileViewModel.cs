using CMS.Services.Token;
using CMS_EF.Models;

namespace CMS.Areas.Admin.ViewModels.File
{
    public class IndexFileViewModel
    {
        public int Page { set; get; }

        public ReflectionIT.Mvc.Paging.PagingList<Files> ListData { set; get; }

        public ITokenService iTokenService { get; set; }
    }
}
