using CMS_Access.Repositories;
using System.Collections.Generic;

namespace CMS.Areas.Admin.ViewModels.ApplicationRole
{
    public class DetailApplicationRoleViewModel
    {
        public CMS_EF.Models.Identity.ApplicationRole Role { get; set; }

        public List<ExtendRoleController> ListRoleControllerAction { get; set; }

    }
}
