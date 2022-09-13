using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models
{
    [Table("NotificationUser")]
    public class NotificationUser
    {
        [Column("Id")]
        public int Id { get; set; }

        [Column("NotificationId")]
        [DefaultValue(0)]
        public int NotificationId { get; set; }

        [Column("SenderId")]
        [DefaultValue(0)]
        public int SenderId { get; set; }

        [Column("ReceiveId")]
        [DefaultValue(0)]
        public int ReceiveId { get; set; }
        
        [Column("IsUnread")]
        [DefaultValue(0)]
        public int IsUnread { get; set; }

        [Column("SenderTime")]
        [DefaultValue("(getdate())")]
        public DateTime SenderTime { get; set; }

        [Column("CreatedAt")]
        [DefaultValue("(getdate())")]
        public DateTime CreatedAt { get; set; }

        [Column("Flag")]
        [DefaultValue(0)]
        public int Flag { get; set; }
    }
}
