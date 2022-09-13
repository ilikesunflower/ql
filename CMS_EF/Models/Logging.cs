using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CMS_EF.Models
{
    public partial class Logging
    {
        public int Id { get; set; }
        public int LogLevel { get; set; }

        [MaxLength(255)]
        public string Ip { get; set; }

        public int UserId { get; set; }

        [MaxLength(255)]
        public string UserAvatar { get; set; }

        [MaxLength(255)]
        public string UserFullName { get; set; }

        public string Action { get; set; }

        public string Detail { get; set; }

        [MaxLength(255)]
        public string UserAgent { get; set; }

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