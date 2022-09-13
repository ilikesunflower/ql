#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CMS_EF.Models.Customers;

namespace CMS_EF.Models.Orders
{
    public class OrderPoint
    {
        [Key] public int Id { get; set; }
        public int OrderId { get; set; }
        public double? Point { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? CustomerPointId { get; set; }
        public int Flag { get; set; }
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("CustomerPointId")]
        [InverseProperty("OrderPoint")]
        public virtual CustomerPoint CustomerPoint { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("OrderPoint")]
        public virtual Orders Order { get; set; }
    }
}