using System.Collections.Generic;
using CMS_EF.Models;

namespace CMS.Areas.Admin.ViewModels.Menus
{
    public class ItemMenuViewModel
    {
        public List<Menu> ListMenus  { get; set; }
        
        public int Pid { get; set; }
    }
}