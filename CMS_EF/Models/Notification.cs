using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models
{
    [Table("Notification")]
    public class Notification
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Title")]
        public string Title { get; set; }

        [Column("Detail")]
        public string Detail { get; set; }

        [Column("Link")]
        public string Link { get; set; }

        [Column("SenderTime")]
        public DateTime SenderTime { get; set; }

        [Column("CreatedBy")]
        [DefaultValue(0)]
        public int CreatedBy { get; set; }

        [Column("CreatedAt")]
        [DefaultValue("(getdate())")]
        public DateTime CreatedAt { get; set; }

        [Column("Flag")]
        [DefaultValue(0)]
        public int Flag { get; set; }
    }
}
