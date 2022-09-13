using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CMS_EF.Models.Identity
{
    public partial class ApplicationUser : IdentityUser<int>
    {
        public override int Id { get; set; }

        public override string Email { get; set; }

        public override string UserName { get; set; }

        public string Description { get; set; }

        public string FullName { get; set; }

        public string Address { get; set; }

        [MaxLength(255)]
        public string Image { get; set; }

        public int Sex { get; set; }

        public DateTime? BirthDay { get; set; }

        [DefaultValue(0)]
        public int Flag { get; set; }

        [DefaultValue(0)]
        public int IsActive { get; set; }
        
        [DefaultValue(0)]
        public int Type { get; set; }

    }
}
