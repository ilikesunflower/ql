#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Customers
{
    public partial class CustomerCouponLog
    {
        [Key]
        public int Id { get; set; }
        public int? CustomerCouponId { get; set; }
        public int? OrderId { get; set; }
        public DateTime? TimeUse { get; set; }
        public int? Status { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int Flag { get; set; }

        [ForeignKey("CustomerCouponId")]
        [InverseProperty("CustomerCouponLog")]
        public virtual CustomerCoupon CustomerCoupon { get; set; }
    }
}