#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Customers
{
    public partial class FeNotification
    {
        public FeNotification()
        {
            FeNotificationCustomer = new HashSet<FeNotificationCustomer>();
        }

        [Key]
        public int Id { get; set; }
        [Column(TypeName = "ntext")]
        public string Title { get; set; }
        [Column(TypeName = "ntext")]
        public string Detail { get; set; }
        public string Link { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int Flag { get; set; }

        [InverseProperty("FeNotification")]
        public virtual ICollection<FeNotificationCustomer> FeNotificationCustomer { get; set; }
    }
}