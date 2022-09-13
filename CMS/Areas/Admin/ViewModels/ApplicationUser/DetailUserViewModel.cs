using CMS_Access.Repositories;
using System;
using System.Collections.Generic;

namespace CMS.Areas.Admin.ViewModels.ApplicationUser
{
    public class DetailUserViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public string Email { get; set; }
        public String PhoneNumber { get; set; }
        public int Sex { get; set; }
        public string TypeLabel { get; set; }
        public int IsActive { get; set; }
        public List<RoleInput> ListRoles { get; set; }
        public bool HasAuthenticator { get; set; }
    }
}
