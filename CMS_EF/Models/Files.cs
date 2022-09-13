using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CMS_EF.Models
{
    public class Files
    {
        public int Id { get; set; }

        // [MaxLength(4000)]
        public string Name { get; set; }

        // [MaxLength(255)]
        public string Url { get; set; }

        // [MaxLength(255)]
        public string Thumbnail { get; set; }

        [MaxLength(255)]
        public string ContentType { get; set; }

        [MaxLength(255)]
        public string Size { get; set; }

        [DefaultValue(0)]
        public int Type { get; set; }

        [DefaultValue("(getdate())")]
        public DateTime CreatedAt { get; set; }

        [DefaultValue(0)]
        public int CreatedBy { get; set; }

        [DefaultValue(0)]
        public int Flag { get; set; }
    }
}
