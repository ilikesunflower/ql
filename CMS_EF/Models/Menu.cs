using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CMS_EF.Models
{
    public partial class Menu
    {
        public int Id { get; set; }

        public int Pid { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string CssClass { get; set; }

        [MaxLength(255)]
        public string Url { get; set; }

        [MaxLength(255)]
        public string ActiveUrl { get; set; }

        [DefaultValue(1)]
        public int Status { get; set; }
        public int? Lvl { get; set; }
        public int? Lft { get; set; }
        public String Rgt { get; set; }

        [DefaultValue("(getdate())")]
        public DateTime CreatedAt { get; set; }

        [DefaultValue(0)]
        public int CreatedBy { get; set; }

        public DateTime LastModifiedAt { get; set; }

        [DefaultValue(0)]
        public int LastModifiedBy { get; set; }

        [DefaultValue(0)]
        public int Flag { get; set; }

        [DefaultValue(0)]
        public int? ControllerId { get; set; }

        [DefaultValue(0)]
        public int? ActionId { get; set; }
    }
}