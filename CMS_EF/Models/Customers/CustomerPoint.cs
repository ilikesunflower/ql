#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.Orders;

namespace CMS_EF.Models.Customers
{
    public partial class CustomerPoint
    {
        public CustomerPoint()
        {
            CustomerPointLog = new HashSet<CustomerPointLog>();
            OrderPoint = new HashSet<OrderPoint>();
        }

        [Key]
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public double? Point { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public int Flag { get; set; }
        public int? PointLock { get; set; }
        
        public double? AddPoint { get; set; }
        public double? MinusPoint { get; set; }
        public int? HistoryFileChargeFileId { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty("CustomerPoint")]
        public virtual Customer Customer { get; set; }
        [InverseProperty("CustomerPoint")]
        public virtual ICollection<CustomerPointLog> CustomerPointLog { get; set; }

        [InverseProperty("CustomerPoint")]
        public virtual ICollection<OrderPoint> OrderPoint { get; set; }
    }
}