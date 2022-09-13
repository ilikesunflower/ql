using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CMS_EF.Models.Identity
{
    public partial class ApplicationUserToken : IdentityUserToken<int>
    {
        [MaxLength(255)]
        public override string LoginProvider { get; set; }

        [MaxLength(255)]
        public override string Name { get; set; }

        [MaxLength(255)]
        public override string Value { get; set; }
    }
}
