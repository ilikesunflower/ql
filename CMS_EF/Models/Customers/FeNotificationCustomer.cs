#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Customers
{
    public partial class FeNotificationCustomer
    {
        [Key]
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public bool? IsRead { get; set; }
        public int? FeNotificationId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Flag { get; set; }

        [ForeignKey("FeNotificationId")]
        [InverseProperty("FeNotificationCustomer")]
        public virtual FeNotification FeNotification { get; set; }
    }
}