#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Customers
{
    public partial class CustomerCoupon
    {
        public CustomerCoupon()
        {
            CustomerCouponLog = new HashSet<CustomerCouponLog>();
        }

        [Key]
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        [StringLength(255)]
        public string Code { get; set; }
        public DateTime? StartTimeUse { get; set; }
        public DateTime? EndTimeUse { get; set; }
        public int? ReducedPrice { get; set; }

        public int? Status { get; set; }
        public int? HistoryFileCoupon { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int Flag { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty("CustomerCoupon")]
        public virtual Customer Customer { get; set; }
        [InverseProperty("CustomerCoupon")]
        public virtual ICollection<CustomerCouponLog> CustomerCouponLog { get; set; }
    }
}