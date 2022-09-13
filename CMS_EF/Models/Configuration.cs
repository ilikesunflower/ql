using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CMS_EF.Models
{
    public partial class Configuration
    {
        public int Id { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }

        [MaxLength(255)]
        public string Val { get; set; }

        public string Detail { get; set; }

        [DefaultValue("(getdate())")]
        public DateTime CreatedAt { get; set; }

        [DefaultValue(0)]
        public int CreatedBy { get; set; }

        public DateTime LastModifiedAt { get; set; }

        [DefaultValue(0)]
        public int LastModifiedBy { get; set; }

        [DefaultValue(0)]
        public int Flag { get; set; }
    }
}