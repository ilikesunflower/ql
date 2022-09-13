using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CMS_EF.Models.Identity
{
    public partial class ApplicationRole : IdentityRole<int>
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }

        public ApplicationRole(string roleName, string description) : base(roleName)
        {
            this.Description = description;
        }
        public string Description { set; get; }

        [MaxLength(255)]
        public override string NormalizedName { get; set; }
    }
}
