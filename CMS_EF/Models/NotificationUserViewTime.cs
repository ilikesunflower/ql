using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models
{
    [Table("NotificationUserViewTime")]
    public class NotificationUserViewTime
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("LastViewTime")]
        public DateTime LastViewTime { get; set; }
    }
}
