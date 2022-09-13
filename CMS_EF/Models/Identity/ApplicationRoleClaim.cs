using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CMS_EF.Models.Identity
{
    public partial class ApplicationRoleClaim : IdentityRoleClaim<int>
    {
        [MaxLength(255)]
        public override string ClaimType { get; set; }

        [MaxLength(255)]
        public override string ClaimValue { get; set; }
    }
}
