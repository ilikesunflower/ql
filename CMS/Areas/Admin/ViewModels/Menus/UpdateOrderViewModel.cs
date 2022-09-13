using System.Collections.Generic;

namespace CMS.Areas.Admin.ViewModels.Menus
{
    public class UpdateOrderViewModel
    {
        public List<int> Ids { set; get; }
        public int Parent { set; get; }
    }
}