#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS_EF.Models.Customers
{
    public partial class CustomerPointLog
    {
        [Key]
        public int Id { get; set; }
        public int? CustomerPointId { get; set; }
        public int? OrderId { get; set; }
        public double? Point { get; set; }
        public int? Status { get; set; }
        public DateTime? TimeUse { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int Flag { get; set; }

        [ForeignKey("CustomerPointId")]
        [InverseProperty("CustomerPointLog")]
        public virtual CustomerPoint CustomerPoint { get; set; }
    }
}