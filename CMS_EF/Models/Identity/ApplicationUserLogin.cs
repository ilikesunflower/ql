using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CMS_EF.Models.Identity
{
    public partial class ApplicationUserLogin : IdentityUserLogin<int>
    {
        [MaxLength(255)]
        public override string LoginProvider { get; set; }

        [MaxLength(255)]
        public override string ProviderKey { get; set; }

        [MaxLength(255)]
        public override string ProviderDisplayName { get; set; }
    }
}
